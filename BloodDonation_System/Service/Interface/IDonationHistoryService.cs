using BloodDonation_System.Model.DTO.Donation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Interface
{
    public interface IDonationHistoryService
    {
        Task<IEnumerable<DonationHistoryDetailDto>> GetAllAsync();
        Task<DonationHistoryDetailDto?> GetByIdAsync(string donationId);
        Task<DonationHistoryDetailDto?> GetDonationHistoryByRequestIdAsync(string requestId);

        Task<IEnumerable<DonationHistoryDetailDto>> GetHistoryByDonorUserIdAsync(string userId);
        Task<DonationHistoryDto> CreateAsync(DonationHistoryDto dto);
        Task<DonationHistoryDetailDto?> UpdateAsync(string donationId, DonationHistoryUpdateDto dto);
        Task<bool> DeleteAsync(string donationId);
    }
}