using BloodDonation_System.Model.DTO.Emergency;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Interface
{
    public interface IEmergencyRequestService
    {
        Task<IEnumerable<EmergencyRequestDto>> GetAllAsync();
        Task<EmergencyRequestDto?> GetByIdAsync(string emergencyId);
        Task<EmergencyRequestDto> CreateAsync(EmergencyRequestDto dto);
        Task<EmergencyRequestDto?> UpdateAsync(string emergencyId, EmergencyRequestDto dto);
        Task<bool> DeleteAsync(string emergencyId);
    }
}
