using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoyalVilla_API.Models.DTOs;
using RoyalVilla_API.Services;

namespace RoyalVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        // Injected AuthService
        private readonly IAuthService _authService = authService;

        [HttpPost("register")]
        [ProducesResponseType(typeof(APIResponse<UserDTO>),StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<UserDTO>>> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            try
            {
                if (registerationRequestDTO == null)
                    return BadRequest(APIResponse<object>.BadRequest("Registration data is required"));

                if (await _authService.IsEmailExistsAsync(registerationRequestDTO.Email))
                    return Conflict(APIResponse<object>.Conflict($"User with email '{registerationRequestDTO.Email}' already exists"));

                var user = await _authService.RegisterAsync(registerationRequestDTO);

                if (user == null)
                    return BadRequest(APIResponse<object>.BadRequest("User registration failed"));

                var response = APIResponse<UserDTO>.CreatedAt(user, "User registered successfully");
                return CreatedAtAction(nameof(Register), response);
            }


            catch (Exception ex)
            {
                var errorReponse = APIResponse<object>.Error(500, "An error occured while registration", ex.Message);
                return StatusCode(500, errorReponse);
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(APIResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<LoginResponseDTO>>> Login(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                if (loginRequestDTO == null)
                    return BadRequest(APIResponse<object>.BadRequest("Login data is required"));

                var loginResponse = await _authService.LoginAsync(loginRequestDTO);

                if (loginResponse == null)
                    return BadRequest(APIResponse<object>.BadRequest("Invalid email or password"));

                var response = APIResponse<LoginResponseDTO>.Ok(loginResponse, "Login successful");

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorReponse = APIResponse<object>.Error(500, "An error occured while login", ex.Message);
                return StatusCode(500, errorReponse);
            }
        }
    }
}
