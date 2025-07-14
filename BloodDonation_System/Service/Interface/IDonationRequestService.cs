using BloodDonation_System.Model.DTO.Donation;
using BloodDonation_System.Model.Enties;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Interface
{
    public interface IDonationRequestService
    {
        Task<IEnumerable<DonationRequestDto>> GetAllAsync();

        Task<DonationRequestDto?> GetByIdAsync(string requestId);

        Task<DonationRequestDto> CreateAsync(DonationRequestInputDto dto);

        Task<DonationRequestDto?> UpdateAsync(string requestId, DonationRequestInputDto dto);

        Task<Dictionary<string, int>> GetSlotCountsByDateAsync(DateOnly date);
        Task<bool> DeleteAsync(string requestId);
        
    }
}
