// DTOs/AuthDTOs/VerifyOtpDto.cs
using System.ComponentModel.DataAnnotations;

namespace RetailAPI.DTOs.AuthDTOs
{
    public class VerifyOtpDto
    {
        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(4), MaxLength(10)]
        public string Otp { get; set; } = string.Empty;
    }
}