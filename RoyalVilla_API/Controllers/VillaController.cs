using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTOs;
using System.Collections.Generic;

namespace RoyalVilla_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer, Admin")]
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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<VillaDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<VillaDTO>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<IEnumerable<VillaDTO>>>> GetVillas()
        {
            var Villas = await _db.Villas.ToListAsync();
            var dtoResponseVilla = _mapper.Map<IEnumerable<VillaDTO>>(Villas);
            var response = APIResponse<IEnumerable<VillaDTO>>.Ok(dtoResponseVilla, "Villas retrieved successfully");
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(APIResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaDTO>>> GetVillaByID(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(APIResponse<object>.BadRequest("Villa ID must be greater than 0"));

                var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

                if (villa == null)
                    return NotFound(APIResponse<object>.NotFound($"Villa with ID {id} was not found"));

                var dtoResponseVilla = _mapper.Map<VillaDTO>(villa);
                return Ok(APIResponse<VillaDTO>.Ok(dtoResponseVilla, "Villa retrieved successfully"));  
            }

            catch (Exception ex)
            {
                var errorReponse = APIResponse<object>.Error(500, "An error occured while creating the villa", ex.Message);
                return StatusCode(500, errorReponse);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<VillaDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaDTO>>> CreateVilla([FromBody] VillaCreateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                    return BadRequest(APIResponse<object>.BadRequest("Villa data is required"));

                var dublicatedVilla = await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == villaDTO.Name.ToLower());
                if (dublicatedVilla != null)
                    return Conflict(APIResponse<object>.Conflict($"A Villa with the name '{villaDTO.Name}' already exists")); // 409 Conflict

                Villa villa = _mapper.Map<Villa>(villaDTO);   

                await _db.Villas.AddAsync(villa);
                await _db.SaveChangesAsync();

                var dtoResponseVilla = _mapper.Map<VillaDTO>(villa);
                var response = APIResponse<VillaDTO>.CreatedAt(dtoResponseVilla, "Villa created successfully");
                return CreatedAtAction(nameof(GetVillaByID), new {id = villa.Id}, response);
            }

            catch (Exception ex)
            {
                var errorReponse = APIResponse<object>.Error(500, "An error occured while creating the villa", ex.Message);
                return StatusCode(500, errorReponse);
            }
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaDTO>>> UpdateVilla([FromBody] VillaUpdateDTO villaDTO, int id)
        {
            try 
            {
                if (villaDTO == null)
                    return BadRequest(APIResponse<object>.BadRequest("Invalid villa data"));

                if (id != villaDTO.Id)
                    return BadRequest(APIResponse<object>.BadRequest("Villa ID in URL doesn't match the Vill ID in request body"));

                var existingVilla = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

                if (existingVilla == null)
                    return NotFound(APIResponse<object>.NotFound($"Villa with ID {id} was not found"));

                var duplicatedVilla = await _db.Villas
                    .FirstOrDefaultAsync(u => u.Name.ToLower() == villaDTO.Name.ToLower() && u.Id != id);

                if (duplicatedVilla != null)
                    return Conflict(APIResponse<object>.Conflict($"A Villa with the name '{villaDTO.Name}' already exists")); // 409 Conflict

                _mapper.Map(villaDTO, existingVilla);
                existingVilla.UpdatedDate = DateTime.Now;

                _db.Villas.Update(existingVilla);
                await _db.SaveChangesAsync();

                var dtoResponseVilla = APIResponse<VillaDTO>.Ok(_mapper.Map<VillaDTO>(villaDTO), "Record updated successfully");
                return Ok(dtoResponseVilla);
            }
            catch (Exception ex)
            {
                var errorReponse = APIResponse<object>.Error(500, "An error occurred while updating the villa", ex.Message);
                return StatusCode(500, errorReponse);
            }

        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<object>>> DeleteVilla(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(APIResponse<object>.BadRequest("Villa ID must be greater than 0"));

                var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

                if (villa == null)
                    return NotFound(APIResponse<object>.NotFound($"Villa with ID {id} was not found"));

                _db.Villas.Remove(villa);
                await _db.SaveChangesAsync();
                return Ok(APIResponse<Villa>.NoContent("Villa deleted successfully"));
            }

            catch (Exception ex)
            {
                var errorReponse = APIResponse<object>.Error(500, "An error occured while creating the villa", ex.Message);
                return StatusCode(500, errorReponse);
            }
        }
    }
}
