using WebsiteBook.Models;

namespace WebsiteBook.Repositories
{
    public interface ICouponRepository
    {
        Task<IEnumerable<Coupon>> GetAllAsync();
        Task<Coupon> GetByIdAsync(int id);
        Task AddAsync(Coupon coupon);
        Task UpdateAsync(Coupon coupon);
        Task DeleteAsync(int id);
        // You can add more methods as per your requirements
    }
}
