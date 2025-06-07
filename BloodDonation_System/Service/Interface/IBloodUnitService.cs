using BloodDonation_System.Model.DTO.Blood;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Interface
{
    public interface IBloodUnitService
    {
        Task<IEnumerable<BloodUnitDto>> GetAllAsync();
        Task<BloodUnitDto?> GetByIdAsync(string unitId);
        Task<BloodUnitDto> CreateAsync(BloodUnitDto dto);
        Task<BloodUnitDto?> UpdateAsync(string unitId, BloodUnitDto dto);
        Task<bool> DeleteAsync(string unitId);
    }
}
