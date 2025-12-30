namespace RoyalVilla_API.Models.DTOs
{
    public class LoginResponseDTO
    {
        public string? Token { get; set; } 

        public UserDTO? UserDTO { get; set; }
    }
}
