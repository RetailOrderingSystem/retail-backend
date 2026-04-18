// Repositories/ProductRepository.cs
using Microsoft.EntityFrameworkCore;
using RetailAPI.Data;
using RetailAPI.DTOs.ProductDTOs;
using RetailAPI.Models;

namespace RetailAPI.Repositories
{
    public class ProductRepository
    {
        private readonly AppDbContext _db;
        public ProductRepository(AppDbContext db) => _db = db;

        public async Task<(List<Product> Items, int Total)> GetFilteredAsync(ProductFilterDto filter)
        {
            var query = _db.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Where(p => p.IsActive);

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(p => p.Name.Contains(filter.Search));

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            if (filter.IsAvailable.HasValue)
                query = query.Where(p => p.IsAvailable == filter.IsAvailable.Value);

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(p => p.Name)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<Product?> GetByIdAsync(int id) =>
            await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.ProductId == id && p.IsActive);

        public async Task<Product> CreateAsync(Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            _db.Products.Update(product);
            await _db.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return false;
            product.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
