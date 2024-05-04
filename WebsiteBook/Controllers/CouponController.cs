using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteBook.Repositories;
using WebsiteBook.Data;
using WebsiteBook.Models;

namespace WebsiteBook.Controllers
{
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICouponRepository _couponRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;


        public CouponController(ICouponRepository couponRepository, ApplicationDbContext context, ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _couponRepository = couponRepository;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _context = context;
        }

        public ActionResult CouponView(ShoppingCart cart)
        {
            ViewBag.ShoppingCart = cart;
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var coupons = await _couponRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            return View((coupons, categories));
        }

        public async Task<IActionResult> ChooseCoupon()
        {
            var coupons = await _couponRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var products = await _productRepository.GetAllAsync();



            return View((coupons, categories, products));
        }

        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> AddAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Admin, Employee")]
        [HttpPost]
        public async Task<IActionResult> Add(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                await _couponRepository.AddAsync(coupon);
                return RedirectToAction(nameof(Index));
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(coupon);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // POST: /Coupon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon == null)
            {
                return NotFound(); // Coupon not found
            }

            await _couponRepository.DeleteAsync(coupon.Id);

            return RedirectToAction(nameof(Index));
        }
        [AllowAnonymous]
        /*        public async Task<IActionResult> Display(int id)
                {
                    var coupon = await _couponRepository.GetByIdAsync(id);
                    var categories = await _categoryRepository.GetAllAsync();

                    if (coupon == null)
                    {
                        return NotFound();
                    }
                    return View(coupon);
                }*/
        public async Task<IActionResult> Display(int id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);

            if (coupon == null)
            {
                return NotFound();
            }

            // Fetch the category associated with the coupon
            var category = await _categoryRepository.GetByIdAsync(coupon.CategoryId);

            if (category == null)
            {
                // Handle the case where the associated category is not found
                return NotFound("Category not found for the coupon.");
            }

            // You can add the category name to the coupon object or create a view model to pass both coupon and category
            coupon.Category.Name = category.Name;

            return View(coupon);
        }


        // Hiển thị form cập nhật sản phẩm
        public async Task<IActionResult> Update(int id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", coupon.CategoryId);

            return View(coupon);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> Update(int id, Coupon coupon)
        {
            if (id != coupon.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                await _couponRepository.UpdateAsync(coupon);
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }
    }
}
