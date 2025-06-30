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

        // ✅ 1. Member phản hồi thông báo
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

        // ✅ 2. Admin/Staff lấy toàn bộ thông báo
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _emergencyNotificationService.GetAllAsync();
            return Ok(list);
        }

        // ✅ 3. Member lấy thông báo theo ID
        [Authorize(Roles = "Member,Admin,Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _emergencyNotificationService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        //// ✅ 4. Admin tạo mới (nếu cần tạo thủ công)
        //[Authorize(Roles = "Admin,Staff")]
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] EmergencyNotificationInputDto dto)
        //{
        //    var created = await _emergencyNotificationService.CreateAsync(dto);
        //    return CreatedAtAction(nameof(GetById), new { id = created.NotificationId }, created);
        //}

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmergencyNotificationInputDto dto)
        {
            var created = await _emergencyNotificationService.CreateAsyncbyStaff(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.NotificationId }, created);
        }

        // ✅ 5. Admin cập nhật
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] EmergencyNotificationDto dto)
        {
            var updated = await _emergencyNotificationService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // ✅ 6. Admin xóa
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _emergencyNotificationService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("by-emergency/{emergencyId}")]
        public async Task<IActionResult> GetByEmergencyId(string emergencyId)
        {
            var result = await _emergencyNotificationService.GetByEmergencyIdAsync(emergencyId);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Staff,Member")]
        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var result = await _emergencyNotificationService.GetByUserIdAsync(userId);
            return Ok(result);
        }

    }
}
