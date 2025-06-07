using BloodDonation_System.Model.DTO.UserProfile;

namespace BloodDonation_System.Service.Interface
{
    public interface IUserProfileService
    {
        Task<UserProfileDto> GetProfileByIdAsync(string profileId);
        Task<IEnumerable<UserProfileDto>> GetAllProfilesAsync();
        Task<UserProfileDto> CreateProfileAsync(CreateUserProfileDto dto);
        Task<UserProfileDto> UpdateProfileAsync(string profileId, UpdateUserProfileDto dto);
        Task<bool> DeleteProfileAsync(string profileId);
    }
}
