using System.ComponentModel.DataAnnotations;

namespace RoyalVilla.DTO
{
    public class RegisterationRequestDTO
    {

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [MaxLength(50)]
        public string Role { get; set; } = "Customer";

    }
}
