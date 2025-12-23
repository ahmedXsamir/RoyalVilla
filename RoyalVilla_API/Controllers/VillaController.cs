using AutoMapper;
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
        private readonly IMapper _mapper;
        public VillaController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            var Villas = await _db.Villas.ToListAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(Villas));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse<VillaDTO>>> GetVillaByID(int id)
        {
            try
            {
                if (id <= 0)
                    return new APIResponse<VillaDTO>()
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Errors = "Villa ID must be greater than 0",
                        Message = "Bad Request"
                    };

                var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

                if (villa == null)
                    return new APIResponse<VillaDTO>()
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Errors = $"Villa with ID {id} was not found",
                        Message = "NotFound"
                    };

                return new APIResponse<VillaDTO>()
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = _mapper.Map<VillaDTO>(villa),
                    Message = "Record retrieved successfully"
                };
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
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                    return NotFound("Villa data is required");

                var dublicatedVilla = await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == villaDTO.Name.ToLower());
                if (dublicatedVilla != null)
                    return Conflict($"A Villa with the name '{villaDTO.Name}' already exists"); // 409 Conflict

                Villa villa = _mapper.Map<Villa>(villaDTO);   

                await _db.Villas.AddAsync(villa);
                await _db.SaveChangesAsync();
                return CreatedAtAction(nameof(GetVillaByID), new { id = villa.Id }, _mapper.Map<VillaDTO>(villa));
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occured while creating the villa : {ex.Message}");
            }
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaUpdateDTO>> UpdateVilla([FromBody] VillaUpdateDTO villaDTO, int id)
        {
            try
            {
                if (villaDTO == null)
                    return BadRequest("Invalid villa data");
                if (id != villaDTO.Id)
                    return BadRequest("Villa ID in URL doesn't match the Vill ID in request body");

                var exisitingVilla = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

                if (exisitingVilla == null)
                    return NotFound($"Villa with ID {id} was not found");

                var dublicatedVilla = await _db.Villas
                    .FirstOrDefaultAsync(u => u.Name.ToLower() == villaDTO.Name.ToLower() && u.Id != id);

                if (dublicatedVilla != null)
                    return Conflict($"A Villa with the name '{villaDTO.Name}' already exists"); // 409 Conflict

                _mapper.Map(villaDTO, exisitingVilla);
                exisitingVilla.UpdatedDate = DateTime.Now;

                _db.Villas.Update(exisitingVilla);
                await _db.SaveChangesAsync();
                return Ok(villaDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occured while updating the villa with ID {id} : {ex.Message}");
            }

        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Villa>> DeleteVilla(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Villa ID must be greater than 0");

                var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

                if (villa == null)
                    return NotFound($"Villa with ID {id} was not found");

                _db.Villas.Remove(villa);
                await _db.SaveChangesAsync();
                return Ok(villa);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occured while deleting the villa with ID {id} : {ex.Message}");
            }
        }
    }
}
