using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTOs;

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

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Villa>> GetVillaByID(int id)
        {
            try
            {
                if (id <= 0)
                    return NotFound("Villa ID must be greater than 0");

                var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

                if (villa == null)
                    return NotFound($"Villa with ID {id} was not found");

                return Ok(villa);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    $"An error occured while retrieving villa with ID {id} : {ex.Message}");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Villa>> CreateVilla([FromBody] VillaCreateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                    return NotFound("Villa data is required");

                Villa villa = new()
                {
                    Name = villaDTO.Name,
                    Details = villaDTO.Details,
                    Rate = villaDTO.Rate,
                    Sqft = villaDTO.Sqft,
                    Occupancy = villaDTO.Occupancy,
                    ImageUrl = villaDTO.ImageUrl,
                    CreatedDate = DateTime.Now
                };

                await _db.Villas.AddAsync(villa);
                await _db.SaveChangesAsync();
                return CreatedAtAction(nameof(GetVillaByID), new { id = villa.Id }, villaDTO);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occured while creating the villa : {ex.Message}");
            }
        }
    }
}
