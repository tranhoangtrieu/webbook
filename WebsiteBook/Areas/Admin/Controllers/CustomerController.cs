using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebsiteBook.Data;
using WebsiteBook.Models;
using WebsiteBook.Repositories;

namespace WebsiteBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)] // Adjust the role as necessary
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICustomerRepository _customerRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public CustomerController(ICustomerRepository customerRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _customerRepository = customerRepository;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString)
        {

            var customer = await _userManager.GetUsersInRoleAsync("Customer");
            var customerIds = customer.Select(u => u.Id);
            var allCustomer = from s in _context.Customer
                              where customerIds.Contains(s.Id)
                              select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                string lowercaseSearchString = searchString.ToLower();
                allCustomer = allCustomer.Where(s => s.FullName.ToLower().Contains(lowercaseSearchString));
            }

            return View(await allCustomer.ToListAsync());
        }

        // GET: Displays the form to update an customer
        public async Task<IActionResult> Edit(string id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Updates an customer
        [HttpPost]
        public async Task<IActionResult> Edit(string id, ApplicationUser customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingCustomer = await _customerRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync

                existingCustomer.FullName = customer.FullName;
                existingCustomer.UserName = customer.UserName;
                existingCustomer.Email = customer.Email;

                await _customerRepository.UpdateAsync(existingCustomer);

                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Displays the confirmation page for deleting an customer
        public async Task<IActionResult> Delete(string id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Deletes an customer
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _customerRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> LockAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Khóa tài khoản
            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            if (result.Succeeded)
            {
                TempData["Message"] = "Khoá Tài Khoản Thành Công";
                return RedirectToAction(nameof(Index));
            }

            return View("Error"); // Hoặc xử lý lỗi phù hợp
        }

        [HttpPost]
        public async Task<IActionResult> UnlockAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            // Mở khóa tài khoản
            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
            if (result.Succeeded)
            {
                TempData["Message"] = "Mở Khoá Tài Khoản Thành Công";
                return RedirectToAction(nameof(Index));
            }
            return View("Error"); // Hoặc xử lý lỗi phù hợp
        }
    }
}
