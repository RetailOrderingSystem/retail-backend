// Repositories/CategoryRepository.cs
using Microsoft.EntityFrameworkCore;
using RetailAPI.Data;
using RetailAPI.Models;

namespace RetailAPI.Repositories
{
    public class CategoryRepository
    {
        private readonly AppDbContext _db;
        public CategoryRepository(AppDbContext db) => _db = db;

        public async Task<List<Category>> GetAllAsync() =>
            await _db.Categories
                .Include(c => c.Products.Where(p => p.IsActive))
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

        public async Task<Category?> GetByIdAsync(int id) =>
            await _db.Categories
                .Include(c => c.Products.Where(p => p.IsActive))
                .FirstOrDefaultAsync(c => c.CategoryId == id && c.IsActive);

        public async Task<Category> CreateAsync(Category category)
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            _db.Categories.Update(category);
            await _db.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return false;
            cat.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
