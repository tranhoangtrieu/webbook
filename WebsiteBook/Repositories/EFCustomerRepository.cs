using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteBook.Data;
using WebsiteBook.Models;

namespace WebsiteBook.Repositories
{
    public class EFCustomerRepository : ICustomerRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;


        public EFCustomerRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            // Query the list of users with the "Customer" role
            var customer = await _userManager.GetUsersInRoleAsync("Customer");
            return customer;
        }

        public async Task<ApplicationUser> GetByIdAsync(string userId)
        {
            // Find a user by their ID
            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }

        public async Task UpdateAsync(ApplicationUser employee)
        {
            // Update a user
            await _userManager.UpdateAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string userId)
        {
            // Delete a user by their ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }
    }
}
