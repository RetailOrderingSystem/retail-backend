// DTOs/AuthDTOs/LoginDto.cs
using System.ComponentModel.DataAnnotations;

namespace RetailAPI.DTOs.AuthDTOs
{
    public class LoginDto
    {
        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;
    }
}