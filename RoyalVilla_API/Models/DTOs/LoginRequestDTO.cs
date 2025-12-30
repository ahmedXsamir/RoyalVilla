using System.ComponentModel.DataAnnotations;

namespace RoyalVilla_API.Models.DTOs
{
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
