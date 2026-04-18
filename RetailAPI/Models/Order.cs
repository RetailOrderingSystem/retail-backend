using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailAPI.Models
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Preparing,
        OutForDelivery,
        Delivered,
        Cancelled
    }

    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public int UserId { get; set; }

        [Required, MaxLength(500)]
        public string DeliveryAddress { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
        public decimal DeliveryFee { get; set; } = 30;
        public decimal Discount { get; set; } = 0;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [MaxLength(200)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
        public int? CouponId { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual Payment? Payment { get; set; }

        [ForeignKey("CouponId")]
        public virtual Coupon? Coupon { get; set; }
    }
}
