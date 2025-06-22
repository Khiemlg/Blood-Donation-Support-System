using BloodDonation_System.Model.DTO.Donation;
using BloodDonation_System.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;

namespace BloodDonation_System.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationHistoryController : ControllerBase
    {
        private readonly IDonationHistoryService _donationHistoryService;

        public DonationHistoryController(IDonationHistoryService donationHistoryService)
        {
            _donationHistoryService = donationHistoryService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DonationHistoryDetailDto>))]
        public async Task<ActionResult<IEnumerable<DonationHistoryDetailDto>>> GetAllDonationHistories()
        {
            var histories = await _donationHistoryService.GetAllAsync();
            return Ok(histories);
        }

        [HttpGet("{donationId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DonationHistoryDetailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DonationHistoryDetailDto>> GetDonationHistoryById(string donationId)
        {
            var history = await _donationHistoryService.GetByIdAsync(donationId);
            if (history == null)
            {
                return NotFound(new { message = "Donation history not found." });
            }
            return Ok(history);
        }

        [HttpGet("by-request/{requestId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DonationHistoryDetailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DonationHistoryDetailDto>> GetHistoryByRequestId(string requestId)
        {
            var donationHistoryDto = await _donationHistoryService.GetDonationHistoryByRequestIdAsync(requestId);

            if (donationHistoryDto == null)
            {
                return NotFound(new { message = "Donation history not found for this request." });
            }

            return Ok(donationHistoryDto);
        }

        [HttpGet("by-donor/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DonationHistoryDetailDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DonationHistoryDetailDto>>> GetDonationHistoryByDonorId(string userId)
        {
            var histories = await _donationHistoryService.GetHistoryByDonorUserIdAsync(userId);

            if (histories == null || !histories.Any())
            {
                return NotFound(new { message = $"Donation history not found for user with ID {userId}." });
            }
            return Ok(histories);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(DonationHistoryDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DonationHistoryDto>> CreateDonationHistory([FromBody] DonationHistoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdHistory = await _donationHistoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetDonationHistoryById), new { donationId = createdHistory.DonationId }, createdHistory);
        }

        [HttpPut("{donationId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DonationHistoryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DonationHistoryDto>> UpdateDonationHistory(string donationId, [FromBody] DonationHistoryUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedHistory = await _donationHistoryService.UpdateAsync(donationId, dto);
            if (updatedHistory == null)
            {
                return NotFound(new { message = "Donation history not found for update." });
            }
            return Ok(updatedHistory);
        }

        [HttpDelete("{donationId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteDonationHistory(string donationId)
        {
            var deleted = await _donationHistoryService.DeleteAsync(donationId);
            if (!deleted)
            {
                return NotFound(new { message = "Donation history not found for deletion." });
            }
            return NoContent();
        }
    }
}