using BloodDonation_System.Model.DTO.Blood;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Interface
{
    public interface IBloodTypeService
    {
        Task<IEnumerable<BloodTypeDto>> GetAllAsync();
        Task<BloodTypeDto?> GetByIdAsync(int id);
        Task<BloodTypeDto> CreateAsync(BloodTypeDto dto);
        Task<BloodTypeDto?> UpdateAsync(int id, BloodTypeDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
