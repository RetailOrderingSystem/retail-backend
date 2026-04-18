using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailAPI.Models
{
    public class OtpLog
    {
        [Key]
        public int OtpId { get; set; }

        public int UserId { get; set; }

        [Required, MaxLength(10)]
        public string OtpCode { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Purpose { get; set; } = string.Empty; // login, reset, verify

        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}

