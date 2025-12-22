using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;

namespace RoyalVilla_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VillaController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public VillaController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Villa>>> GetVillas()
        {
            List<Villa> Villas = await _db.Villas.ToListAsync();
            return Ok(Villas);
        }

    }
}
