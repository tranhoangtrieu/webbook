using WebsiteBook.Models;

namespace WebsiteBook.Repositories
{
    public interface ITacGia
    {

        Task<IEnumerable<TacGia>> GetAllAsync();
        Task<TacGia> GetByIdAsync(int id);
        Task AddAsync(TacGia tacGia);
        Task UpdateAsync(TacGia tacGia);
        Task DeleteAsync(int id);
    }
}
