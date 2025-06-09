// File: YourProject.Controllers.DonationHistoryController.cs
// (Không thay đổi so với lần cuối bạn cung cấp)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

// Đảm bảo các using cần thiết:
using BloodDonation_System.Data; // DButils của bạn
using BloodDonation_System.Model.DTO.Donation; // DonationHistoryDto, DonationHistoryCreationDto
using BloodDonation_System.Model.Enties; // DonationHistory, DonationRequest
using BloodDonation_System.Service.Interface; // IDonationHistoryService

[Route("api/[controller]")]
[ApiController]
public class DonationHistoryController : ControllerBase
{
    private readonly DButils _context;
    private readonly IDonationHistoryService _donationHistoryService;

    public DonationHistoryController(DButils context, IDonationHistoryService donationHistoryService)
    {
        _context = context;
        _donationHistoryService = donationHistoryService;
    }

    // Các phương thức cơ bản CRUD cho DonationHistory (GetAllAsync, GetByIdAsync, CreateAsync, DeleteAsync) sẽ ở đây...

    // *** Phương thức GET để lấy lịch sử bằng Request ID (được sử dụng bởi UI) ***
    /// <summary>
    /// Lấy một bản ghi lịch sử hiến máu theo ID Yêu cầu hiến máu liên quan của nó.
    /// </summary>
    [HttpGet("by-request/{requestId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DonationHistoryDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DonationHistoryDto>> GetHistoryByRequestId(string requestId)
    {
        var dh = await _context.DonationHistories
                               .FirstOrDefaultAsync(h => h.DonationRequestId.Equals(requestId));

        if (dh == null) return NotFound(new { message = "Không tìm thấy lịch sử hiến máu cho yêu cầu này." });

        // Ánh xạ sang DTO
        return Ok(new DonationHistoryDto
        {
            DonationId = dh.DonationId,
            DonorUserId = dh.DonorUserId,
            DonationDate = dh.DonationDate,
            BloodTypeId = dh.BloodTypeId,
            ComponentId = dh.ComponentId,
            QuantityMl = dh.QuantityMl,
            EligibilityStatus = dh.EligibilityStatus,
            ReasonIneligible = dh.ReasonIneligible,
            TestingResults = dh.TestingResults,
            StaffUserId = dh.StaffUserId,
            Status = dh.Status,
            EmergencyId = dh.EmergencyId,
            Descriptions = dh.Descriptions,
            DonationRequestId = dh.DonationRequestId
        });
    }

    // *** Phương thức PUT để cập nhật lịch sử (được sử dụng bởi UI) ***
    /// <summary>
    /// Cập nhật một bản ghi lịch sử hiến máu hiện có.
    /// </summary>
    [HttpPut("{donationId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DonationHistoryDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DonationHistoryDto>> UpdateHistory(
        string donationId,
        [FromBody] DonationHistoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedDto = await _donationHistoryService.UpdateAsync(donationId, dto);

        if (updatedDto == null)
        {
            return NotFound(new { message = "Không tìm thấy lịch sử hiến máu." });
        }

        return Ok(updatedDto);
    }
}