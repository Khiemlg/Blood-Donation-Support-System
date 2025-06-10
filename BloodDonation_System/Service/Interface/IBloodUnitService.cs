// File: BloodDonation_System.Service.Interface.IBloodUnitService.cs

using BloodDonation_System.Model.DTO.Blood;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Interface
{
    public interface IBloodUnitService
    {
        // Basic CRUD operations returning the base BloodUnitDto for input/output
        Task<IEnumerable<BloodUnitDto>> GetAllAsync();
        Task<BloodUnitDto?> GetByIdAsync(string unitId);
        Task<BloodUnitDto> CreateAsync(BloodUnitDto dto);
        Task<BloodUnitDto?> UpdateAsync(string unitId, BloodUnitDto dto);
        Task<bool> DeleteAsync(string unitId);

        // New methods for inventory-specific detailed output
        // These methods return BloodUnitInventoryDto for richer information including related names.
        Task<IEnumerable<BloodUnitInventoryDto>> GetInventoryUnitsAsync();
        Task<BloodUnitInventoryDto?> GetInventoryUnitByIdAsync(string unitId);
    }
}