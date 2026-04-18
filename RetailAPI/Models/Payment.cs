using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailAPI.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        [MaxLength(50)]
        public string Method { get; set; } = string.Empty; // Cash, Card, UPI, Wallet

        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Success, Failed, Refunded

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [MaxLength(255)]
        public string? TransactionId { get; set; }

        public DateTime PaidAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;
    }
}
