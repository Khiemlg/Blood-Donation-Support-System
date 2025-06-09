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

        //[HttpPost("create")]
        //[Authorize(Roles = "Staff")]
        //public async Task<IActionResult> Create([FromBody] EmergencyRequestCreateDto dto)
        //{
        //    // Lấy user_id từ token JWT
        //    var userId = User.FindFirstValue("user_id");


        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized("User not identified.");

        //    var result = await _service.CreateEmergencyRequestAsync(dto, userId);

        //    if (!result.Success)
        //        return BadRequest(new { message = result.Message });

        //    return Ok(new { message = result.Message });
        //}

        // ban test tạm BỎ [Authorize] và gán staffUserId thủ công for test

        [HttpPost("create")]
        // [Authorize(Roles = "Staff")] // Tạm thời bỏ để test
        public async Task<IActionResult> Create([FromBody] EmergencyRequestCreateDto dto)
        {
            var userId = "STAFF_001"; // Gán cứng userID tồn tại trong DB chưa có user_id thật từ JWT token sau khi đăng nhập

            var result = await _service.CreateEmergencyRequestAsync(dto, userId);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }



        //------------------------


    }
}
