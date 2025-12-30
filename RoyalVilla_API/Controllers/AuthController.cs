using Microsoft.AspNetCore.Mvc;
using RoyalVilla_API.Models.DTOs;

namespace RoyalVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public AuthController()
        {
            
        }

        [HttpPost("register")]
        public async Task<ActionResult<APIResponse<UserDTO>>> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            var response = APIResponse<UserDTO>.Ok(null!, "User registered successfully");
            return Ok(response);
        }
    }
}
