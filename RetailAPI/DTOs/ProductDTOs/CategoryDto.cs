// DTOs/ProductDTOs/CategoryDto.cs
using System.ComponentModel.DataAnnotations;

namespace RetailAPI.DTOs.ProductDTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public int ProductCount { get; set; }
    }

    public class CreateCategoryDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class UpdateCategoryDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsActive { get; set; }
    }
}
