using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailAPI.Models
{
    public class LoyaltyPoint
    {
        [Key]
        public int LoyaltyId { get; set; }

        public int UserId { get; set; }

        public int Points { get; set; } = 0;

        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // Earned, Redeemed

        [MaxLength(255)]
        public string? Description { get; set; }

        public int? OrderId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}


