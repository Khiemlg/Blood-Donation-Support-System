using BloodDonation_System.Model.DTO.UserProfile;

namespace BloodDonation_System.Service.Interface
{
    public interface IUserProfileService
    {
        /// <summary>
        /// Lấy hồ sơ theo ID
        /// </summary>
        Task<UserProfileDto?> GetProfileByUserIdAsync(string userId);

        /// <summary>
        /// Lấy toàn bộ hồ sơ
        /// </summary>
        Task<IEnumerable<UserProfileDto>> GetAllProfilesAsync();

        /// <summary>
        /// Tạo hồ sơ mới (Create)
        /// </summary>
        Task<UserProfileDto> CreateProfileAsync(CreateUserProfileDto dto);

        /// <summary>
        /// Cập nhật hồ sơ cá nhân (Update)
        /// </summary>
        Task<UserProfileDto?> UpdateProfileByUserIdAsync(string userId, UpdateUserProfileDto dto);


        /// <summary>
        /// Xóa hồ sơ cá nhân
        /// </summary>
        Task<bool> DeleteProfileByUserIdAsync(string userId);

    }
}
 