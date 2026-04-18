// DTOs/AuthDTOs/RegisterDto.cs
using System.ComponentModel.DataAnnotations;

namespace RetailAPI.DTOs.AuthDTOs
{
    public class RegisterDto
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6), MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(15)]
        public string? Phone { get; set; }

        // Default Address (required for registration)
        [Required, MaxLength(255)]
        public string Street { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string ZipCode { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Country { get; set; }
    }
}