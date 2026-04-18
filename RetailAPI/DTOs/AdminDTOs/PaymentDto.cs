using System;
using System.ComponentModel.DataAnnotations;

namespace RetailAPI.DTOs.AdminDTOs
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? TransactionId { get; set; }
        public DateTime PaidAt { get; set; }
    }

    public class CreatePaymentDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        [RegularExpression("Cash|Card|UPI|Wallet")]
        public string Method { get; set; } = string.Empty;

        [Required, Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public string? TransactionId { get; set; }
    }

    public class UpdatePaymentStatusDto
    {
        [Required]
        [RegularExpression("Pending|Success|Failed|Refunded")]
        public string Status { get; set; } = string.Empty;
    }
}
