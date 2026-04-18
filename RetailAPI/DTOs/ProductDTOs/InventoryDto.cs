// DTOs/ProductDTOs/InventoryDto.cs
namespace RetailAPI.DTOs.ProductDTOs
{
    public class InventoryDto
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int LowStockThreshold { get; set; }
        public bool IsLowStock => Quantity <= LowStockThreshold;
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateInventoryDto
    {
        public int Quantity { get; set; }
        public int? LowStockThreshold { get; set; }
    }
}
