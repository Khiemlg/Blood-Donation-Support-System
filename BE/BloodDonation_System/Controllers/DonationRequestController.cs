// File: BloodDonation_System.Controllers.DonationRequestController.cs

using BloodDonation_System.Model.DTO.Donation;
using BloodDonation_System.Model.Enties;
using BloodDonation_System.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks; // Thêm using này

namespace BloodDonation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationRequestController : ControllerBase
    {
        private readonly IDonationRequestService _donationRequestService;

        public DonationRequestController(IDonationRequestService donationRequestService)
        {
            _donationRequestService = donationRequestService;
        }

        // --- Tạo Yêu Cầu Hiến Máu (POST) ---
        [HttpPost]
        [Route("RegisterDonationRequest")]
        [AllowAnonymous] // Hoặc [Authorize(Roles = "Donor")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DonationRequestDto))] // Trả về DTO input (hoặc DTO sau khi tạo nếu bạn muốn)
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterDonationRequest([FromBody] DonationRequestInputDto Dto) // NHẬN DonationRequestDto (CHỈ CÓ ID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _donationRequestService.CreateAsync(Dto);
                return Ok(result); // Service trả về DonationRequestDto, OK
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi khi đăng ký yêu cầu hiến máu.", details = ex.Message });
            }
        }

        // --- Lấy Tất Cả Yêu Cầu (GET) ---
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DonationRequestDto>))] // TRẢ VỀ DonationRequestDetailDto (CÓ TÊN)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllDonationRequests()
        {
            try
            {
                var requests = await _donationRequestService.GetAllAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi khi lấy danh sách yêu cầu hiến máu.", details = ex.Message });
            }
        }

        // --- Lấy Yêu Cầu Theo ID (GET) ---
        [HttpGet("{requestId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DonationRequestDto))] // TRẢ VỀ DonationRequestDetailDto (CÓ TÊN)
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDonationRequestById(string requestId)
        {
            try
            {
                var request = await _donationRequestService.GetByIdAsync(requestId);
                if (request == null)
                {
                    return NotFound($"Không tìm thấy yêu cầu hiến máu với ID: {requestId}");
                }
                return Ok(request);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Đã xảy ra lỗi khi lấy yêu cầu hiến máu ID {requestId}.", details = ex.Message });
            }
        }

        // --- Cập Nhật Yêu Cầu (PUT) ---
        [HttpPut("{requestId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DonationRequest))] // TRẢ VỀ DonationRequestDetailDto (CÓ TÊN)
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDonationRequest(string requestId, [FromBody] DonationRequestInputDto dto) // NHẬN DonationRequestDto (CHỈ CÓ ID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedRequest = await _donationRequestService.UpdateAsync(requestId, dto);

                if (updatedRequest == null)
                {
                    return NotFound($"Không tìm thấy yêu cầu hiến máu với ID: {requestId}");
                }

                return Ok(updatedRequest); // Service trả về DonationRequestDetailDto, OK
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = "Lỗi xung đột dữ liệu.", details = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi khi cập nhật yêu cầu hiến máu.", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi không mong muốn.", details = ex.Message });
            }
        }

        // --- Xóa Yêu Cầu (DELETE) ---
        [HttpDelete("{requestId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDonationRequest(string requestId)
        {
            try
            {
                var deleted = await _donationRequestService.DeleteAsync(requestId);
                if (!deleted)
                {
                    return NotFound($"Không tìm thấy yêu cầu hiến máu với ID: {requestId}");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Đã xảy ra lỗi khi xóa yêu cầu hiến máu ID {requestId}.", details = ex.Message });
            }
        }
    }
}