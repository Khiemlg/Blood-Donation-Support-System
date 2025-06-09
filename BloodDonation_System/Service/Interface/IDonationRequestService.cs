using BloodDonation_System.Model.DTO.Donation;
using System.Collections.Generic;
using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc; // Loại bỏ using này vì Service không nên trả về ActionResult<T>

namespace BloodDonation_System.Service.Interface
{
    public interface IDonationRequestService
    {
        // Lấy tất cả yêu cầu hiến máu, trả về danh sách DonationRequestResponseDto
        Task<IEnumerable<DonationRequestDto>> GetAllAsync();

        // Lấy yêu cầu hiến máu theo ID, trả về DonationRequestResponseDto
        Task<DonationRequestDto?> GetByIdAsync(string requestId);

        // Tạo mới yêu cầu hiến máu, nhận DonationRequestInputDto và trả về DonationRequestResponseDto
        Task<DonationRequestDto> CreateAsync(DonationRequestInputDto dto);

        // Cập nhật yêu cầu hiến máu, nhận DonationRequestInputDto và trả về DonationRequestResponseDto
        Task<DonationRequestDto?> UpdateAsync(string requestId, DonationRequestInputDto dto);

        // Xóa yêu cầu hiến máu
        Task<bool> DeleteAsync(string requestId);
    }
}
