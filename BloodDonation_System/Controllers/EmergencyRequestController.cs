using BloodDonation_System.Model.DTO.Emergency;
using BloodDonation_System.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonation_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmergencyRequestController : ControllerBase
    {
        private readonly IEmergencyRequestService _service;

        public EmergencyRequestController(IEmergencyRequestService service)
        {
            _service = service;
        }


        [HttpPost("create")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Create([FromBody] EmergencyRequestCreateDto dto)
        {
           
            var userId = User.FindFirst("user_id")?.Value;


            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not identified.");

            var result = await _service.CreateEmergencyRequestAsync(dto, userId);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });

        }

        [HttpGet("list")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] string? status = null)
        {
            var requests = await _service.GetAllEmergencyRequestsAsync(status);
            return Ok(requests);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> GetById(string id)
        {
            var request = await _service.GetEmergencyRequestByIdAsync(id);
            if (request == null)
                return NotFound(new { message = "Request not found" });
            return Ok(request);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] string status)
        {
            var result = await _service.UpdateEmergencyRequestStatusAsync(id, status);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpPost("{id}/notify-donors")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> NotifyDonors(string id)
        {
            var result = await _service.NotifyDonorsForEmergencyAsync(id);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpGet("my-requests")]
        [Authorize]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = User.FindFirst("user_id")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not identified.");

            var requests = await _service.GetEmergencyRequestsByUserAsync(userId);
            return Ok(requests);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> CancelRequest(string id)
        {
            var result = await _service.CancelEmergencyRequestAsync(id);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }








    }
}
