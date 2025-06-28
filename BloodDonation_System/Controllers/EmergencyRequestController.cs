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

        //tạm thời khoá chức năng này do chưa có user_id thật từ JWT token sau khi đăng nhập

        [HttpPost("create")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Create([FromBody] EmergencyRequestCreateDto dto)
        {
            // Lấy claim 'sub' chứa user_id
            //var userId = User.FindFirst("sub")?.Value;
            var userId = User.FindFirst("user_id")?.Value;


            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not identified.");

            var result = await _service.CreateEmergencyRequestAsync(dto, userId);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });

        }

        // Lấy danh sách tất cả yêu cầu máu khẩn cấp (có thể lọc theo trạng thái)
        [HttpGet("list")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] string? status = null)
        {
            var requests = await _service.GetAllEmergencyRequestsAsync(status);
            return Ok(requests);
        }

        // Lấy chi tiết một yêu cầu máu khẩn cấp theo ID
        [HttpGet("{id}")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> GetById(string id)
        {
            var request = await _service.GetEmergencyRequestByIdAsync(id);
            if (request == null)
                return NotFound(new { message = "Request not found" });
            return Ok(request);
        }

        // Cập nhật trạng thái yêu cầu máu khẩn cấp
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] string status)
        {
            var result = await _service.UpdateEmergencyRequestStatusAsync(id, status);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        // Gửi thông báo khẩn cấp tới các donor phù hợp
        [HttpPost("{id}/notify-donors")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> NotifyDonors(string id)
        {
            var result = await _service.NotifyDonorsForEmergencyAsync(id);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        // Xem lịch sử yêu cầu máu khẩn cấp của người dùng hiện tại
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

        // Hủy yêu cầu máu khẩn cấp
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
