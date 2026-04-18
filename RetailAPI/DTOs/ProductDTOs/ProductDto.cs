using System.ComponentModel.DataAnnotations;

namespace RetailAPI.DTOs.ProductDTOs
{
    // ── Response DTO ──
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public bool IsLowStock { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ── Create DTO ──
    public class CreateProductDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0.01, 99999.99)]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;

        [Required]
        public int CategoryId { get; set; }

        public int InitialStock { get; set; } = 0;
        public int LowStockThreshold { get; set; } = 10;
    }

    // ── Update DTO ──
    public class UpdateProductDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }
        public string? Description { get; set; }

        [Range(0.01, 99999.99)]
        public decimal? Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsAvailable { get; set; }
        public int? CategoryId { get; set; }
    }

    // ── Filter DTO ──
    public class ProductFilterDto
    {
        public int? CategoryId { get; set; }
        public string? Search { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsAvailable { get; set; }
        public int? LocationId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }

    // ── Paged Result ──
    public class PagedProductDto
    {
        public List<ProductDto> Products { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}

