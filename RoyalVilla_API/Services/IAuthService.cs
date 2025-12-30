using RoyalVilla_API.Models.DTOs;

namespace RoyalVilla_API.Services
{
    public interface IAuthService
    {
        Task<UserDTO?> RegisterAsync(RegisterationRequestDTO registerationRequestDTO);
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<bool> IsEmailExistsAsync(string email);
    }
}
