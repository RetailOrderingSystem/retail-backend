// Services/InventoryService.cs
using RetailAPI.DTOs.ProductDTOs;
using RetailAPI.Repositories;

namespace RetailAPI.Services
{
    public class InventoryService
    {
        private readonly InventoryRepository _repo;
        public InventoryService(InventoryRepository repo) => _repo = repo;

        public async Task<InventoryDto?> GetByProductIdAsync(int productId)
        {
            var inv = await _repo.GetByProductIdAsync(productId);
            if (inv == null) return null;
            return new InventoryDto
            {
                InventoryId = inv.InventoryId,
                ProductId = inv.ProductId,
                ProductName = inv.Product?.Name ?? "",
                Quantity = inv.Quantity,
                LowStockThreshold = inv.LowStockThreshold,
                UpdatedAt = inv.UpdatedAt
            };
        }

        public async Task<List<InventoryDto>> GetLowStockAsync()
        {
            var list = await _repo.GetLowStockAsync();
            return list.Select(inv => new InventoryDto
            {
                InventoryId = inv.InventoryId,
                ProductId = inv.ProductId,
                ProductName = inv.Product?.Name ?? "",
                Quantity = inv.Quantity,
                LowStockThreshold = inv.LowStockThreshold,
                UpdatedAt = inv.UpdatedAt
            }).ToList();
        }

        public async Task<InventoryDto> UpdateAsync(int productId, UpdateInventoryDto dto)
        {
            var inv = await _repo.UpsertAsync(productId, dto.Quantity, dto.LowStockThreshold);
            return new InventoryDto
            {
                InventoryId = inv.InventoryId,
                ProductId = inv.ProductId,
                Quantity = inv.Quantity,
                LowStockThreshold = inv.LowStockThreshold,
                UpdatedAt = inv.UpdatedAt
            };
        }
    }
}

