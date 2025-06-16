using BloodDonation_System.Model.DTO.Emergency;
using BloodDonation_System.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmergencyNotificationController : ControllerBase
    {
        private readonly IEmergencyNotificationService _emergencyNotificationService;

        public EmergencyNotificationController(IEmergencyNotificationService emergencyNotificationService)
        {
            _emergencyNotificationService = emergencyNotificationService;
        }

        [Authorize(Roles = "Member")]
        [HttpPost("respond")]
        public async Task<IActionResult> RespondToEmergency([FromBody] EmergencyResponseDTO dto)
        {
            var userId = User.FindFirstValue("user_id");

            if (userId == null)
                return Unauthorized("Không tìm thấy thông tin người dùng");

            var result = await _emergencyNotificationService.RespondToEmergencyNotificationAsync(userId, dto);

            if (result.StartsWith("❌") || result.StartsWith("⚠️"))
                return BadRequest(new { message = result });

            return Ok(new { message = result });
        }

    }
}
