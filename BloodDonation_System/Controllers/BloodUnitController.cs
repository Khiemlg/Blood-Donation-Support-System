using BloodDonation_System.Model.DTO.Blood;
using BloodDonation_System.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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
        /// Cập nhật trạng thái đơn vị máu thành "Discarded" và lý do loại bỏ
        /// </summary>
        [HttpPost("discard/{bloodUnitId}")]
        public async Task<IActionResult> DiscardBloodUnit(string bloodUnitId, [FromBody] DiscardBloodUnitDto dto)
        {
            try
            {
                
                var result = await _bloodUnitService.DiscardBloodUnitAsync(bloodUnitId, dto.DiscardReason);

                // Nếu thành công
                if (result)
                {
                    return Ok(new { message = "Đơn vị máu đã được loại bỏ thành công." });
                }
                else
                {
                    return BadRequest(new { message = "Không thể loại bỏ đơn vị máu, trạng thái không hợp lệ." });
                }
            }
            catch (Exception ex)
            {
              
                return BadRequest(new { error = ex.Message });
            }
        }












        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BloodUnitInventoryDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<BloodUnitInventoryDto>>> GetAllBloodUnits()
        {
            try
            {
                var units = await _bloodUnitService.GetAllAsync();
                return Ok(units);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in GetAllBloodUnits: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving blood units." });
            }
        }

        [HttpGet("{unitId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BloodUnitInventoryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BloodUnitInventoryDto>> GetBloodUnitById(string unitId)
        {
            try
            {
                var unit = await _bloodUnitService.GetByIdAsync(unitId);
                if (unit == null)
                {
                    return NotFound(new { message = $"Blood unit with ID '{unitId}' not found." });
                }
                return Ok(unit);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in GetBloodUnitById (ID: {unitId}): {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the blood unit." });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BloodUnitInventoryDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BloodUnitInventoryDto>> CreateBloodUnit([FromBody] BloodUnitDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdUnit = await _bloodUnitService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetBloodUnitById), new { unitId = createdUnit.UnitId }, createdUnit);
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine($"Validation error in CreateBloodUnit: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                Console.Error.WriteLine($"Operation error in CreateBloodUnit: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to create blood unit due to a service operation error.", details = ex.Message });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unhandled error in CreateBloodUnit: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while creating the blood unit." });
            }
        }

        [HttpPut("{unitId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BloodUnitInventoryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BloodUnitInventoryDto>> UpdateBloodUnit(string unitId, [FromBody] BloodUnitDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedUnit = await _bloodUnitService.UpdateAsync(unitId, dto);
                if (updatedUnit == null)
                {
                    return NotFound(new { message = $"Blood unit with ID '{unitId}' not found for update." });
                }
                return Ok(updatedUnit);
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine($"Validation error in UpdateBloodUnit (ID: {unitId}): {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                Console.Error.WriteLine($"Operation error in UpdateBloodUnit (ID: {unitId}): {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to update blood unit due to a service operation error.", details = ex.Message });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unhandled error in UpdateBloodUnit (ID: {unitId}): {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while updating the blood unit." });
            }
        }

        [HttpDelete("{unitId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteBloodUnit(string unitId)
        {
            try
            {
                var deleted = await _bloodUnitService.DeleteAsync(unitId);
                if (!deleted)
                {
                    return NotFound(new { message = $"Blood unit with ID '{unitId}' not found for deletion." });
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                Console.Error.WriteLine($"Deletion error in DeleteBloodUnit (ID: {unitId}): {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unhandled error in DeleteBloodUnit (ID: {unitId}): {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while deleting the blood unit." });
            }
        }

        [HttpGet("bybloodtype/{bloodTypeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BloodUnitInventoryDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<BloodUnitInventoryDto>>> GetBloodUnitsByBloodTypeId(int bloodTypeId)
        {
            try
            {
                var units = await _bloodUnitService.GetByBloodTypeIdAsync(bloodTypeId);
                return Ok(units);
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine($"Validation error in GetBloodUnitsByBloodTypeId (ID: {bloodTypeId}): {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unhandled error in GetBloodUnitsByBloodTypeId (ID: {bloodTypeId}): {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while retrieving blood units by blood type." });
            }
        }

        [HttpPost("separate/{unitId}")]
        public async Task<IActionResult> SeparateBloodUnit(string unitId)
        {
            try
            {
                var result = await _bloodUnitService.SeparateBloodUnitAsync(unitId);
                if (!result)
                    return BadRequest(new { success = false, message = "Không thể tách máu. Đơn vị máu không tồn tại, không ở trạng thái 'Separating' hoặc không phải máu toàn phần." });

                return Ok(new { success = true, message = "Tách máu thành công." });
            }
            catch (Exception ex)
            {
                // Log lỗi tại đây nếu cần
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi khi tách máu.", error = ex.Message });
            }
        }
    }
}