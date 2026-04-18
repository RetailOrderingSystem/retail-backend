using System;
using System.ComponentModel.DataAnnotations;

namespace RetailAPI.DTOs.AdminDTOs
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public int? MaxUsageCount { get; set; }
        public int UsedCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCouponDto
    {
        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [RegularExpression("Percentage|Fixed")]
        public string DiscountType { get; set; } = "Percentage";

        [Required, Range(0.01, double.MaxValue)]
        public decimal DiscountValue { get; set; }

        public decimal? MinOrderAmount { get; set; }
        public int? MaxUsageCount { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class UpdateCouponDto
    {
        [RegularExpression("Percentage|Fixed")]
        public string? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public int? MaxUsageCount { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class ValidateCouponDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;
        public decimal OrderAmount { get; set; }
    }
}
