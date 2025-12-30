using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoyalVilla_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;   
        private readonly IConfiguration _configuration;
        public AuthService(ApplicationDbContext db, IMapper mapper, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _configuration = configuration;
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

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(user => user.Email.ToLower() == loginRequestDTO.Email.ToLower());

                if (user == null || user.Password != loginRequestDTO.Password)
                    throw new UnauthorizedAccessException("Invalid email or password.");

                var token = GenerateJwtToken(user);

                return new LoginResponseDTO
                {
                    UserDTO = _mapper.Map<UserDTO>(user),
                    Token = token
                };
            }

            catch (Exception ex)
            {

                throw new InvalidOperationException("An unexpected error occurred during user login", ex);
            }
        }

        private string GenerateJwtToken(User user)
        {
            // Create security key using the secret key from configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is not configured.")));

            // Create signing credentials using the security key and HMAC SHA256 algorithm
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create token descriptor
            // it contains the claims, expiration time, and signing credentials for the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Define the claims for the token
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),

                // Set token expiration time to 7 days from now
                Expires = DateTime.UtcNow.AddDays(7),

                // Set the signing credentials
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
