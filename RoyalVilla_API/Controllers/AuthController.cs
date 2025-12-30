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
        [ProducesResponseType(typeof(APIResponse<UserDTO>), 201)]
        [ProducesResponseType(typeof(APIResponse<object>), 400)]
        [ProducesResponseType(typeof(APIResponse<object>), 409)]
        [ProducesResponseType(typeof(APIResponse<object>), 500)]
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
    }
}
