using BloodDonation_System.Model.DTO.Emergency;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Interface
{
    public interface IEmergencyRequestService
    {
        // code để tạo yêu cầu máu khẩn cấp từ Staff 8/6/-15h code by khiem
        Task<(bool Success, string Message)> CreateEmergencyRequestAsync(EmergencyRequestCreateDto dto, string staffUserId);
        // -----------------------------------------
        Task<IEnumerable<EmergencyRequestDto>> GetAllAsync();
        Task<EmergencyRequestDto?> GetByIdAsync(string emergencyId);
        Task<EmergencyRequestDto> CreateAsync(EmergencyRequestDto dto);
        Task<EmergencyRequestDto?> UpdateAsync(string emergencyId, EmergencyRequestDto dto);
        Task<bool> DeleteAsync(string emergencyId);
    }
}
