using BloodDonation_System.Model.DTO.Emergency;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Interface
{
    public interface IEmergencyRequestService
    {
        Task<(bool Success, string Message)> CreateEmergencyRequestAsync(EmergencyRequestCreateDto dto, string staffUserId);
        
        Task<IEnumerable<EmergencyRequestDto>> GetAllEmergencyRequestsAsync(string? status);

        Task<EmergencyRequestDto?> GetEmergencyRequestByIdAsync(string emergencyId);

        Task<(bool Success, string Message)> UpdateEmergencyRequestStatusAsync(string emergencyId, string status);

        Task<(bool Success, string Message)> NotifyDonorsForEmergencyAsync(string emergencyId);

        Task<IEnumerable<EmergencyRequestDto>> GetEmergencyRequestsByUserAsync(string userId);

        Task<(bool Success, string Message)> CancelEmergencyRequestAsync(string emergencyId);
        Task<IEnumerable<EmergencyRequestDto>> GetAllAsync();
        Task<EmergencyRequestDto?> GetByIdAsync(string emergencyId);
        Task<EmergencyRequestDto> CreateAsync(EmergencyRequestDto dto);
        Task<EmergencyRequestDto?> UpdateAsync(string emergencyId, EmergencyRequestDto dto);
        Task<bool> DeleteAsync(string emergencyId);
    }
}
