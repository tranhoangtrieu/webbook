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
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public EmployeeController(IEmployeeRepository employeeRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _employeeRepository = employeeRepository;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString)
        {

            var employee = await _userManager.GetUsersInRoleAsync("Employee");
            var employeeIds = employee.Select(u => u.Id);
            var allEmployee = from s in _context.Employee
                              where employeeIds.Contains(s.Id)
                              select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                string lowercaseSearchString = searchString.ToLower();
                allEmployee = allEmployee.Where(s => s.FullName.ToLower().Contains(lowercaseSearchString));
            }

            return View(await allEmployee.ToListAsync());
        }

        // GET: Displays the form to update an employee
        public async Task<IActionResult> Edit(string id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Updates an employee
        [HttpPost]
        public async Task<IActionResult> Edit(string id, ApplicationUser employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingEmployee = await _employeeRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync

                existingEmployee.FullName = employee.FullName;
                existingEmployee.UserName = employee.UserName;
                existingEmployee.Email = employee.Email;

                await _employeeRepository.UpdateAsync(existingEmployee);

                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Displays the confirmation page for deleting an employee
        public async Task<IActionResult> Delete(string id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Deletes an employee
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _employeeRepository.DeleteAsync(id);
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