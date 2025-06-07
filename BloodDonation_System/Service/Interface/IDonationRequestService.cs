using BloodDonation_System.Model.DTO.Donation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Interface
{
    public interface IDonationRequestService
    {
        Task<IEnumerable<DonationRequestDto>> GetAllAsync();
        Task<DonationRequestDto?> GetByIdAsync(string requestId);
        Task<DonationRequestDto> CreateAsync(DonationRequestDto dto);
        Task<DonationRequestDto?> UpdateAsync(string requestId, DonationRequestDto dto);
        Task<bool> DeleteAsync(string requestId);
    }
}
