using BloodDonation_System.Model.DTO.Blood;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Interface
{
    public interface IBloodComponentService
    {
        Task<IEnumerable<BloodComponentDto>> GetAllAsync();
        Task<BloodComponentDto?> GetByIdAsync(int id);
        Task<BloodComponentDto> CreateAsync(BloodComponentDto dto);
        Task<BloodComponentDto?> UpdateAsync(int id, BloodComponentDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
