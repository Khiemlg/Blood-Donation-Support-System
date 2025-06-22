using BloodDonation_System.Model.DTO.UserProfile;

namespace BloodDonation_System.Service.Interface
{
    public interface ISearchDonorService
    {
        Task<IEnumerable<UserProfileDto>> SearchSuitableDonorsAsync(SearchDonorDto searchDto);
    }

}
