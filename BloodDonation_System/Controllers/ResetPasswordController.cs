using BloodDonation_System.Model.DTO.Auth;
using BloodDonation_System.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResetPasswordController : ControllerBase
    {
        private readonly IResetPasswordService _resetService;

        public ResetPasswordController(IResetPasswordService resetService)
        {
            _resetService = resetService;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] ForgotPasswordDto dto)
        {
            var success = await _resetService.SendOtpToEmailAsync(dto.Email);
            return success ? Ok("OTP sent successfully.") : NotFound("Email not found.");
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var success = await _resetService.ResetPasswordAsync(dto);
            return success ? Ok("Password reset successful.") : BadRequest("Invalid or expired OTP.");
        }
    }

}
