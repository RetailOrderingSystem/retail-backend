// Repositories/InventoryRepository.cs
using Microsoft.EntityFrameworkCore;
using RetailAPI.Data;
using RetailAPI.Models;

namespace RetailAPI.Repositories
{
    public class InventoryRepository
    {
        private readonly AppDbContext _db;
        public InventoryRepository(AppDbContext db) => _db = db;

        public async Task<Inventory?> GetByProductIdAsync(int productId) =>
            await _db.Inventories
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.ProductId == productId);

        public async Task<List<Inventory>> GetLowStockAsync() =>
            await _db.Inventories
                .Include(i => i.Product)
                .Where(i => i.Quantity <= i.LowStockThreshold)
                .ToListAsync();

        public async Task<Inventory> UpsertAsync(int productId, int quantity, int? threshold = null)
        {
            var inv = await _db.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId);
            if (inv == null)
            {
                inv = new Inventory
                {
                    ProductId = productId,
                    Quantity = quantity,
                    LowStockThreshold = threshold ?? 10
                };
                _db.Inventories.Add(inv);
            }
            else
            {
                inv.Quantity = quantity;
                if (threshold.HasValue) inv.LowStockThreshold = threshold.Value;
                inv.UpdatedAt = DateTime.UtcNow;
            }
            // Auto-disable product if out of stock
            var product = await _db.Products.FindAsync(productId);
            if (product != null) product.IsAvailable = quantity > 0;
            await _db.SaveChangesAsync();
            return inv;
        }
    }
}
