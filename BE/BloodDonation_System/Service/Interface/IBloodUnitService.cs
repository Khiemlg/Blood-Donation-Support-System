// File: BloodDonation_System.Service.Interface.IBloodUnitService.cs

using BloodDonation_System.Model.DTO.Blood;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Interface
{
    public interface IBloodUnitService
    {
        Task<IEnumerable<BloodUnitInventoryDto>> GetAllAsync();

        Task<BloodUnitInventoryDto?> GetByIdAsync(string unitId);

        Task<BloodUnitInventoryDto> CreateAsync(BloodUnitDto dto);

        Task<BloodUnitInventoryDto?> UpdateAsync(string unitId, BloodUnitDto dto);

        Task<bool> DeleteAsync(string unitId);

        Task<IEnumerable<BloodUnitInventoryDto>> GetByBloodTypeIdAsync(int bloodTypeId);
    }
}