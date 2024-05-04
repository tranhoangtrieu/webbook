using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using WebsiteBook.Repositories;
using WebsiteBook.Data;
using WebsiteBook.Models;


namespace WebsiteBook.Controllers
{


    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly ITacGia _tacGia;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, ApplicationDbContext context, ICouponRepository couponRepository, ITacGia tacGia)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _couponRepository = couponRepository;
            _context = context;
            _tacGia = tacGia;
        }

        public async Task<IActionResult> Index(string? searchString, string? categoryName, string sortOrder)
        {
            var allProducts = _context.Products.AsQueryable();
            var allCategories = _context.Categories.AsQueryable();
            var allCoupons = _context.Coupon.AsQueryable();
            var allTacGia = _context.TacGia.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                string lowercaseSearchString = searchString.ToLower();
                allProducts = allProducts.Where(p => p.Name.ToLower().Contains(lowercaseSearchString));
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                allProducts = allProducts.Where(p => p.Category.Name.ToLower() == categoryName.ToLower());
            }

            // Search theo tăng giảm giá
            switch (sortOrder)
            {
                case "price_asc":
                    allProducts = allProducts.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    allProducts = allProducts.OrderByDescending(p => p.Price);
                    break;
                default:
                    break;
            }

            // Kiểm tra nếu là role của Customer
            if (User.IsInRole("Customer"))
            {
                // Lọc ra những sản phẩm không bị vô hiệu hoa
                allProducts = allProducts.Where(p => !p.IsDetactive);
            }

            // Fetch all data
            var products = await allProducts.ToListAsync();
            var categories = await allCategories.ToListAsync();
            var coupons = await allCoupons.ToListAsync();
            var tacgia = await allTacGia.ToListAsync(); 
            var viewModel = new ProductCategoryViewModel
            {
                Products = products,
                Categories = categories,
                Coupons = coupons,
                TacGia= tacgia
            };

            return View(viewModel);
        }


        [HttpPost]
        public JsonResult GetSearchValue(string search)
        {
            var productResult = _context.Products.Where(x => x.Name.Contains(search))
                                        .Select(x => new
                                        {
                                            label = x.Name,
                                            value = x.Name,
                                        }).ToList();
            return Json(productResult);
        }
        [HttpPost]
        public JsonResult GetSearchCategoryValue(string search)
        {
            var productResult = _context.Categories.Where(x => x.Name.Contains(search))
                                        .Select(x => new
                                        {
                                            label = x.Name,
                                            value = x.Name,
                                        }).ToList();
            return Json(productResult);
        }


        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> AddAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            var tacgia = await _tacGia.GetAllAsync();
            ViewBag.TacGia = new SelectList(tacgia, "Id", "Name");

            return View();
        }

        // Add product
        [Authorize(Roles = "Admin, Employee")]
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl, List<IFormFile> imageUrls)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // Lưu hình ảnh đại diện
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                if (imageUrls != null)
                {
                    product.ImageUrls = new List<string>();
                    foreach (var file in imageUrls)
                    {
                        // Lưu các hình ảnh khác
                        product.ImageUrls.Add(await SaveImage(file));
                    }
                }

                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            var tacgia = await _tacGia.GetAllAsync();
            ViewBag.TacGia = new SelectList(tacgia, "Id", "Name");

            return View(product);
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName); // Thay đổi đường dẫn theo cấu hình của bạn
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName; // Trả về đường dẫn tương đối
        }
        private async Task<Product> GetProductFromDatabase(int productId)
        {
            // Truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm
            var product = await _productRepository.GetByIdAsync(productId);
            return product;
        }

        // Hiển thị thông tin chi tiết sản phẩm
        public async Task<IActionResult> Display(int productId)
        {
            if (productId == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            // Check if the user is in the "Customer" role and the product is inactive
            if (User.IsInRole("Customer") && product.IsDetactive == true)
            {
                // Redirect the customer to a page that explains the product is not available
                return RedirectToAction("ProductNotAvailable", "Home");
            }

            return View(product);
        }



        // Hiển thị form cập nhật sản phẩm
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                await _productRepository.UpdateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }


        // Hiển thị form xác nhận xóa sản phẩm
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> DeleteAll()
        {
            var products = await _productRepository.GetAllAsync();
            if (products == null)
            {
                return NotFound();
            }
            else
            {
                foreach (var product in products)
                {
                    await _productRepository.DeleteAsync(product.Id);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> DeactivateProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.IsDetactive = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]

        public async Task<IActionResult> ReactivateProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.IsDetactive = false;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
