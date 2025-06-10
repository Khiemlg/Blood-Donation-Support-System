using BloodDonation_System.Model.DTO.Blood; // Make sure this namespace is correct for your DTOs
using BloodDonation_System.Service.Interface; // Make sure this namespace is correct for your service interface
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodDonation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodUnitController : ControllerBase
    {
        private readonly IBloodUnitService _bloodUnitService;

        public BloodUnitController(IBloodUnitService bloodUnitService)
        {
            _bloodUnitService = bloodUnitService;
        }

        /// <summary>
        /// Retrieves all blood units with basic information.
        /// </summary>
        /// <returns>A collection of BloodUnitDto.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BloodUnitDto>))]
        public async Task<ActionResult<IEnumerable<BloodUnitDto>>> GetAllBloodUnits()
        {
            var units = await _bloodUnitService.GetAllAsync();
            return Ok(units);
        }

        /// <summary>
        /// Retrieves a single blood unit by its ID with basic information.
        /// </summary>
        /// <param name="unitId">The ID of the blood unit.</param>
        /// <returns>A BloodUnitDto if found, otherwise 404 Not Found.</returns>
        [HttpGet("{unitId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BloodUnitDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BloodUnitDto>> GetBloodUnitById(string unitId)
        {
            var unit = await _bloodUnitService.GetByIdAsync(unitId);
            if (unit == null)
            {
                return NotFound(new { message = "Blood unit not found." });
            }
            return Ok(unit);
        }

        /// <summary>
        /// Creates a new blood unit.
        /// </summary>
        /// <param name="dto">The BloodUnitDto containing the data for the new unit.</param>
        /// <returns>The created BloodUnitDto if successful, otherwise 400 Bad Request.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BloodUnitDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BloodUnitDto>> CreateBloodUnit([FromBody] BloodUnitDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdUnit = await _bloodUnitService.CreateAsync(dto);
            // Returns 201 Created with the location of the new resource
            return CreatedAtAction(nameof(GetBloodUnitById), new { unitId = createdUnit.UnitId }, createdUnit);
        }

        /// <summary>
        /// Updates an existing blood unit.
        /// </summary>
        /// <param name="unitId">The ID of the blood unit to update.</param>
        /// <param name="dto">The BloodUnitDto containing the updated data.</param>
        /// <returns>The updated BloodUnitDto if successful, 404 Not Found if the unit doesn't exist, or 400 Bad Request.</returns>
        [HttpPut("{unitId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BloodUnitDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BloodUnitDto>> UpdateBloodUnit(string unitId, [FromBody] BloodUnitDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedUnit = await _bloodUnitService.UpdateAsync(unitId, dto);
            if (updatedUnit == null)
            {
                return NotFound(new { message = "Blood unit not found for update." });
            }
            return Ok(updatedUnit);
        }

        /// <summary>
        /// Deletes a blood unit by its ID.
        /// </summary>
        /// <param name="unitId">The ID of the blood unit to delete.</param>
        /// <returns>204 No Content if successful, or 404 Not Found if the unit doesn't exist.</returns>
        [HttpDelete("{unitId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteBloodUnit(string unitId)
        {
            var deleted = await _bloodUnitService.DeleteAsync(unitId);
            if (!deleted)
            {
                return NotFound(new { message = "Blood unit not found for deletion." });
            }
            return NoContent();
        }

        /// <summary>
        /// Retrieves all blood units with detailed inventory information (including BloodType and Component names).
        /// </summary>
        /// <returns>A collection of BloodUnitInventoryDto.</returns>
        [HttpGet("inventory")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BloodUnitInventoryDto>))]
        public async Task<ActionResult<IEnumerable<BloodUnitInventoryDto>>> GetInventoryUnits()
        {
            var units = await _bloodUnitService.GetInventoryUnitsAsync();
            return Ok(units);
        }

        /// <summary>
        /// Retrieves a single blood unit by its ID with detailed inventory information.
        /// </summary>
        /// <param name="unitId">The ID of the blood unit.</param>
        /// <returns>A BloodUnitInventoryDto if found, otherwise 404 Not Found.</returns>
        [HttpGet("inventory/{unitId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BloodUnitInventoryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BloodUnitInventoryDto>> GetInventoryUnitById(string unitId)
        {
            var unit = await _bloodUnitService.GetInventoryUnitByIdAsync(unitId);
            if (unit == null)
            {
                return NotFound(new { message = "Blood unit not found in inventory." });
            }
            return Ok(unit);
        }
    }
}