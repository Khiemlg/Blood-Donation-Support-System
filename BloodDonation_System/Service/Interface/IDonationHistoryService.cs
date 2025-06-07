using BloodDonation_System.Model.DTO.Donation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Interface
{
    public interface IDonationHistoryService
    {
        Task<IEnumerable<DonationHistoryDto>> GetAllAsync();
        Task<DonationHistoryDto?> GetByIdAsync(string donationId);
        Task<DonationHistoryDto> CreateAsync(DonationHistoryDto dto);
        Task<DonationHistoryDto?> UpdateAsync(string donationId, DonationHistoryDto dto);
        Task<bool> DeleteAsync(string donationId);
    }
}
