using Microsoft.EntityFrameworkCore;
using WebsiteBook.Data;
using WebsiteBook.Models;

namespace WebsiteBook.Repositories
{
    public class ECouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _context;

        public ECouponRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Coupon> GetByIdAsync(int id)
        {
            return await _context.Coupon.FindAsync(id);
        }

        public async Task<IEnumerable<Coupon>> GetAllAsync()
        {
            return await _context.Coupon.ToListAsync();
        }

        public async Task AddAsync(Coupon coupon)
        {
            _context.Coupon.Add(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Coupon coupon)
        {
            _context.Entry(coupon).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var coupon = await GetByIdAsync(id);
            _context.Coupon.Remove(coupon);
            await _context.SaveChangesAsync();
        }
    }
}
