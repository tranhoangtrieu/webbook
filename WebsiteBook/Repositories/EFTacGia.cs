using Microsoft.EntityFrameworkCore;
using WebsiteBook.Data;
using WebsiteBook.Models;

namespace WebsiteBook.Repositories
{
    public class EFTacGia : ITacGia
    {
        private readonly ApplicationDbContext _context;
        public EFTacGia(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(TacGia tacGia)
        {
            _context.TacGia.Add(tacGia);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tacGia = await _context.TacGia.FindAsync(id);
            _context.TacGia.Remove(tacGia);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TacGia>> GetAllAsync()
        {
            return await _context.TacGia.ToListAsync();
        }

        public async Task<TacGia> GetByIdAsync(int id)
        {
            return await _context.TacGia.FindAsync(id);
        }

        public async Task UpdateAsync(TacGia tacGia)
        {
            _context.TacGia.Update(tacGia);
            await _context.SaveChangesAsync();
        }
    }
}
