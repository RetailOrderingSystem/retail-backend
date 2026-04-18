// Services/ProductService.cs
using RetailAPI.DTOs.ProductDTOs;
using RetailAPI.Models;
using RetailAPI.Repositories;

namespace RetailAPI.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepo;
        private readonly InventoryRepository _inventoryRepo;

        public ProductService(ProductRepository productRepo, InventoryRepository inventoryRepo)
        {
            _productRepo = productRepo;
            _inventoryRepo = inventoryRepo;
        }

        public async Task<PagedProductDto> GetProductsAsync(ProductFilterDto filter)
        {
            var (items, total) = await _productRepo.GetFilteredAsync(filter);
            return new PagedProductDto
            {
                Products = items.Select(MapToDto).ToList(),
                TotalCount = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var p = await _productRepo.GetByIdAsync(id);
            return p == null ? null : MapToDto(p);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                IsAvailable = dto.InitialStock > 0 && dto.IsAvailable,
                CategoryId = dto.CategoryId
            };
            var created = await _productRepo.CreateAsync(product);
            await _inventoryRepo.UpsertAsync(created.ProductId, dto.InitialStock, dto.LowStockThreshold);
            return MapToDto(await _productRepo.GetByIdAsync(created.ProductId) ?? created);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return null;
            if (dto.Name != null) product.Name = dto.Name;
            if (dto.Description != null) product.Description = dto.Description;
            if (dto.Price.HasValue) product.Price = dto.Price.Value;
            if (dto.ImageUrl != null) product.ImageUrl = dto.ImageUrl;
            if (dto.IsAvailable.HasValue) product.IsAvailable = dto.IsAvailable.Value;
            if (dto.CategoryId.HasValue) product.CategoryId = dto.CategoryId.Value;
            await _productRepo.UpdateAsync(product);
            return MapToDto(product);
        }

        public async Task<bool> DeleteProductAsync(int id) =>
            await _productRepo.DeleteAsync(id);

        private static ProductDto MapToDto(Product p) => new()
        {
            ProductId = p.ProductId,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            IsAvailable = p.IsAvailable,
            IsActive = p.IsActive,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? "",
            StockQuantity = p.Inventory?.Quantity ?? 0,
            IsLowStock = p.Inventory != null && p.Inventory.Quantity <= p.Inventory.LowStockThreshold,
            CreatedAt = p.CreatedAt
        };
    }
}
