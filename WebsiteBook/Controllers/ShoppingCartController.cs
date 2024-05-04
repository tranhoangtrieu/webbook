using WebsiteBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using WebsiteBook.Data;
using WebsiteBook.Extensions;
using WebsiteBook.Models;
using WebsiteBook.Repositories;
using WebsiteBook.Services;

namespace WebsiteBook.Controllers
{
    [Authorize(Roles = SD.Role_Customer)]

    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVnPayService _vnPayservice;
        public ShoppingCartController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IVnPayService vnPayservice,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ICouponRepository couponRepository)
        {
            _context = context;
            _userManager = userManager;
            _vnPayservice = vnPayservice;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _couponRepository = couponRepository;
        }

        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {

            // lấy thông tin sản phẩm từ productId
            var product = await GetProductFromDatabase(productId);
            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                CategoryId = product.CategoryId,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Quantity = quantity
            };
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // kiểm tra Url trước kia
            string previousRoute = Request.Headers["Referer"].ToString();

            // Quay về Url trước kia
            if (!string.IsNullOrEmpty(previousRoute))
            {
                return Redirect(previousRoute);
            }
            else
            {
                // Ko thấy Url trước kia sẽ quay về Home.
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult ChooseCoupon(ChooseCouponViewModel model, string action)
        {
            if (action == "AddSelectedCoupon")
            {
                // Update the shopping cart with the selected coupons
                foreach (var coupon in model.SelectedCoupons.Values)
                {
                    // Add the coupon to the shopping cart
                }
                return RedirectToAction("Index", "ShoppingCart");
            }
            // Other logic for handling the form submission
            return View(model);
        }

        public async Task<IActionResult> AddCouponToCart(int Id)
        {
            var coupon = await _context.Coupon.FindAsync(Id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        /*        public async Task<IActionResult> GetCategoryFromShoppingCart()
                {
                    var coupons = await _couponRepository.GetAllAsync();
                    var categories = await _categoryRepository.GetAllAsync();

                    // Retrieve categories of products from the shopping cart (Assuming you have a ShoppingCartService)
                    var shoppingCartCategories = await _shoppingCartService.GetCategoriesAsync();

                    // Pass the coupons, categories, and shopping cart categories to the view
                    return View(new ValueTuple<IEnumerable<Coupon>, IEnumerable<Category>, IEnumerable<Category>>(coupons, categories, shoppingCartCategories));
                }*/


        public async Task<IActionResult> Index(int[] selectedCouponIds)
        {
            var allProducts = _context.Products.AsQueryable();
            var allCoupons = _context.Coupon.AsQueryable();
            var allCategories = _context.Categories.AsQueryable();

            // Retrieve the selected coupons
            var selectedCoupons = await allCoupons.Where(c => selectedCouponIds.Contains(c.Id)).ToListAsync();

            // Fetch all data
            var products = await allProducts.ToListAsync();
            var categories = await allCategories.ToListAsync();
            var coupons = await allCoupons.ToListAsync();

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            //return View((cart, selectedCoupons, products, categories));
            return View(new Tuple<ShoppingCart, IEnumerable<Coupon>, IEnumerable<Product>, IEnumerable<Category>>(cart, selectedCoupons, products, categories));
        }

        private async Task<Product> GetProductFromDatabase(int productId)
        {
            // Truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm
            var product = await _productRepository.GetByIdAsync(productId);
            return product;
        }

        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart is not null)
            {
                cart.RemoveItem(productId);
                // Lưu lại giỏ hàng vào Session sau khi đã xóa mục
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveAllFromCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            // Clear the entire cart logic here (e.g., remove all items from the cart)
            if (cart is not null)
            {
                HttpContext.Session.Remove("Cart");
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            // Kiểm tra nếu productId và quantity hợp lệ
            if (productId > 0 && quantity > 0)
            {
                // Lấy giỏ hàng từ Session
                var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

                // Tìm sản phẩm trong giỏ hàng
                var itemToUpdate = cart.Items.FirstOrDefault(item => item.ProductId == productId);

                // Kiểm tra xem sản phẩm có tồn tại không
                if (itemToUpdate != null)
                {
                    // Cập nhật số lượng sản phẩm
                    itemToUpdate.Quantity = quantity;

                    // Lưu giỏ hàng đã cập nhật vào Session
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                }
            }

            // Chuyển hướng lại trang giỏ hàng
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> OrderList()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = _context.Orders
                                 .Where(o => o.UserId == user.Id)
                                 .ToList();

            if (!orders.Any())
            {
                // Xử lý khi không có đơn hàng nào
                ViewBag.Message = "Bạn chưa có đơn hàng nào.";
                return View();
            }

            // Tạo ViewModel nếu muốn truyền thêm dữ liệu hoặc thông tin lý giải cho View
            return View(orders); // Gởi danh sách đơn hàng đến View
        }
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = _context.Orders
                                .Include(o => o.OrderDetails)
                                .ThenInclude(od => od.Product)
                                .FirstOrDefault(o => o.Id == orderId && o.UserId == user.Id);

            if (order == null)
            {
                // Xử lý trường hợp không tìm thấy đơn hàng
                return RedirectToAction("OrderList");
            }

            // Trả về View với model là order đã tìm được
            return View(order);
        }

        //hiển thị thông tin sản phẩm
        public async Task<IActionResult> Display(int productId)
        {
            var product = await GetProductFromDatabase(productId);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Checkout(Order order, decimal discountTotal)
        {
            if (string.IsNullOrWhiteSpace(order.Notes))
            {
                order.Notes = "No notes provided";
            }
            if (ModelState.IsValid)
            {
                order.TotalPrice = discountTotal;

                _context.Orders.Update(order);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View((new Order(), discountTotal));
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order, string payment, decimal discountTotal)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                // Xử lý giỏ hàng trống...
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.TotalPrice = discountTotal;
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList();
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            if (payment == "Thanh toán VNPay")
            {
                var vnPayModel = new VnPaymentRequest
                {
                    Amount = (double)order.TotalPrice,
                    CreatedDate = DateTime.Now,

                    Description = user.FullName + user.PhoneNumber,
                    FullName = user.FullName,
                    OrderId = order.Id


                };
                var ID = vnPayModel.OrderId;
                return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
            }
            HttpContext.Session.Remove("Cart");
            return View("OrderCompleted", order.Id); // Trang xác nhận hoàn thành đơn hàng
        }
        [Authorize]
        public IActionResult PaymentSuccess()
        {
            HttpContext.Session.Remove("Cart");
            return View();
        }
        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }
        [Authorize]
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayservice.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }


            // Lưu đơn hàng vô database

            TempData["Message"] = $"Thanh toán VNPay thành công";


            return RedirectToAction("PaymentSuccess");
        }
    }
}