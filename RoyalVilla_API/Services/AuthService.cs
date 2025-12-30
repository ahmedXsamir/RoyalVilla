using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTOs;

namespace RoyalVilla_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;   
        public AuthService(ApplicationDbContext db, IMapper mapper, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            // Check if any user exists with the given email (case-insensitive)
            return await _db.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<UserDTO?> RegisterAsync(RegisterationRequestDTO registerationRequestDTO)
        {
            try
            {
                // Check if email already exists
                if (await IsEmailExistsAsync(registerationRequestDTO.Email))
                    throw new InvalidOperationException($"User with email '{registerationRequestDTO.Email}' already exists");

                // Create new user entity
                User user = new()
                {
                    Email = registerationRequestDTO.Email,
                    Name = registerationRequestDTO.Name,
                    Password = registerationRequestDTO.Password,
                    Role = (string.IsNullOrEmpty(registerationRequestDTO.Role)) ? "Customer" : registerationRequestDTO.Role,
                    CreatedDate = DateTime.UtcNow,
                };

                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();

                // Map to UserDTO and return
                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException("An unexpected error occurred during user registration", ex);
            }
        }

        public Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            throw new NotImplementedException();
        }

    }
}
