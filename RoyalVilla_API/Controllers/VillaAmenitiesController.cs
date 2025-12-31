using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTOs;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RoyalVilla_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VillaAmenitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public VillaAmenitiesController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<VillaAmenitiesDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<VillaAmenitiesDTO>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<IEnumerable<VillaAmenitiesDTO>>>> GetAllVillaAmenities()
        {
            var villaAmenities = await _db.VillaAmenities.ToListAsync();
            var villaAmenitiesDTO = _mapper.Map<List<VillaAmenitiesDTO>>(villaAmenities);
            var response = APIResponse<IEnumerable<VillaAmenitiesDTO>>.Ok(villaAmenitiesDTO, "Villa amenities retrieved successfully");

            return Ok(response);
        }

        [HttpGet("{id:int}", Name = "GetVillaAmenitiesByID")]
        [ProducesResponseType(typeof(APIResponse<VillaAmenitiesDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaAmenitiesDTO>>> GetVillaAmenitiesByID(int id)
        {
            try
            {
                if (id <= 0)
                    return NotFound(APIResponse<VillaAmenitiesDTO>.NotFound("Villa ID must be greater than 0"));

                var villaAmenities = await _db.VillaAmenities.FirstOrDefaultAsync(villaAmenities => villaAmenities.Id == id);

                if (villaAmenities == null)
                    return NotFound(APIResponse<VillaAmenitiesDTO>.NotFound($"Villa amenities with ID {id} not found"));

                var villaAmenitiesDTO = _mapper.Map<VillaAmenitiesDTO>(villaAmenities);
                return Ok(APIResponse<VillaAmenitiesDTO>.Ok(villaAmenitiesDTO, "Villa amenities retrieved successfully"));
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An error occurred while processing your request", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<VillaAmenitiesCreateDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaAmenitiesDTO>>> CreateVillaAmenities([FromBody] VillaAmenitiesCreateDTO villaAmenitiesCreateDTO)
        {
            try
            {
                if (villaAmenitiesCreateDTO == null)
                    return BadRequest(APIResponse<object>.BadRequest("Villa amenities data is required"));

                var villaExists = await _db.Villas.FirstOrDefaultAsync(v => v.Id == villaAmenitiesCreateDTO.VillaId);

                if (villaExists == null)
                    return Conflict(APIResponse<object>.Conflict($"Villa with the ID '{villaAmenitiesCreateDTO.VillaId}'does not exist."));

                var villaAmentites = _mapper.Map<VillaAmenities>(villaAmenitiesCreateDTO);
                villaAmentites.CreatedDate = DateTime.UtcNow;

                await _db.VillaAmenities.AddAsync(villaAmentites);
                await _db.SaveChangesAsync();

                var responseDTO = APIResponse<VillaAmenitiesDTO>.CreatedAt(_mapper.Map<VillaAmenitiesDTO>(villaAmentites), "Villa amenities created successfully");

                return CreatedAtAction(nameof(GetVillaAmenitiesByID), new { id = villaAmentites.Id }, responseDTO);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An error occurred while creating the villa amenities", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<VillaAmenitiesDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaAmenitiesDTO>>> UpdateVillaAmenities([FromBody] VillaAmenitiesUpdateDTO villaAmenitiesUpdateDTO, int id)
        {
            try
            {
                if (villaAmenitiesUpdateDTO == null)
                    return BadRequest(APIResponse<object>.BadRequest("Villa amenities data is required"));

                if (villaAmenitiesUpdateDTO.Id != id)
                    return BadRequest(APIResponse<object>.BadRequest("Villa Amenities ID in URL doesn't match the Vill Amenities ID in request body"));

                var villaExists = await _db.Villas.FirstOrDefaultAsync(villa => villa.Id == villaAmenitiesUpdateDTO.VillaId);

                if (villaExists == null)
                    return Conflict(APIResponse<object>.Conflict($"Villa with the ID '{villaAmenitiesUpdateDTO.VillaId}'does not exist."));

                var villaAmenitiesExists = await _db.VillaAmenities.FirstOrDefaultAsync(villaAmenities => villaAmenities.Id == id);

                if (villaAmenitiesExists == null)
                    return Conflict(APIResponse<object>.Conflict($"Villa Amenities with the ID '{id}'does not exist."));

                _mapper.Map(villaAmenitiesUpdateDTO, villaAmenitiesExists);
                villaAmenitiesExists.UpdatedDate = DateTime.UtcNow;

                _db.VillaAmenities.Update(villaAmenitiesExists);
                await _db.SaveChangesAsync();

                var response = APIResponse<VillaAmenitiesDTO>.Ok(_mapper.Map<VillaAmenitiesDTO>(villaAmenitiesExists), "villa amenities updated successfully");

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An error occurred while updating the villa amenities", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaAmenitiesDTO>>> DeleteVillaAmenities(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(APIResponse<object>.BadRequest("villa amenities ID must be greater than 0"));

                var villaAmentites = await _db.VillaAmenities.FirstOrDefaultAsync(villaAmenities => villaAmenities.Id == id);

                if (villaAmentites == null)
                    return NotFound(APIResponse<object>.NotFound($"villa amenities with ID '{id}' was not found"));

                _db.VillaAmenities.Remove(villaAmentites);
                await _db.SaveChangesAsync();

                var response = APIResponse<VillaAmenitiesDTO>.NoContent("Villa Amenities deleted successfully");
                return Ok(response);
            }

            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An error occurred while deleting the villa amenities", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }       
    }
}
