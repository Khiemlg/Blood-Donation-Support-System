using BloodDonation_System.Model.DTO.UserProfile;

namespace BloodDonation_System.Service.Interface
{
    public interface IUserProfileService
    {
       
        Task<UserProfileDto?> GetProfileByUserIdAsync(string userId);

        
        Task<IEnumerable<UserProfileDto>> GetAllProfilesAsync();

        
        Task<UserProfileDto> CreateProfileAsync(CreateUserProfileDto dto);

      
        Task<UserProfileDto?> UpdateProfileByUserIdAsync(string userId, UpdateUserProfileDto dto);

        Task<bool> DeleteProfileByUserIdAsync(string userId);

    }
}
 