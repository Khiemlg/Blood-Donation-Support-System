using BloodDonation_System.Model.DTO.Emergency;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Interface
{
    public interface IEmergencyRequestService
    {
        // code để tạo yêu cầu máu khẩn cấp từ Staff 8/6/-15h code by khiem
        Task<(bool Success, string Message)> CreateEmergencyRequestAsync(EmergencyRequestCreateDto dto, string staffUserId);
        // Tạo yêu cầu máu khẩn cấp
        
        // Lấy tất cả yêu cầu máu khẩn cấp, có thể lọc theo trạng thái
        Task<IEnumerable<EmergencyRequestDto>> GetAllEmergencyRequestsAsync(string? status);

        // Lấy chi tiết một yêu cầu máu khẩn cấp theo ID
        Task<EmergencyRequestDto?> GetEmergencyRequestByIdAsync(string emergencyId);

        // Cập nhật trạng thái yêu cầu máu khẩn cấp
        Task<(bool Success, string Message)> UpdateEmergencyRequestStatusAsync(string emergencyId, string status);

        // Gửi thông báo khẩn cấp tới các donor phù hợp
        Task<(bool Success, string Message)> NotifyDonorsForEmergencyAsync(string emergencyId);

        // Lấy danh sách yêu cầu máu khẩn cấp của một user
        Task<IEnumerable<EmergencyRequestDto>> GetEmergencyRequestsByUserAsync(string userId);

        // Hủy yêu cầu máu khẩn cấp
        Task<(bool Success, string Message)> CancelEmergencyRequestAsync(string emergencyId);
        // -----------------------------------------
        Task<IEnumerable<EmergencyRequestDto>> GetAllAsync();
        Task<EmergencyRequestDto?> GetByIdAsync(string emergencyId);
        Task<EmergencyRequestDto> CreateAsync(EmergencyRequestDto dto);
        Task<EmergencyRequestDto?> UpdateAsync(string emergencyId, EmergencyRequestDto dto);
        Task<bool> DeleteAsync(string emergencyId);
    }
}
