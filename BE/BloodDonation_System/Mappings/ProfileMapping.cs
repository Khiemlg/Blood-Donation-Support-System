using BloodDonation_System.Model.DTO.User;
using BloodDonation_System.Model.Enties;

namespace BloodDonation_System.Mappings
{
    public class ProfileMapping
    {
        public UserDto MapUserToDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email ?? string.Empty,
                RoleId = user.RoleId,
                RoleName = user.Role?.RoleName ?? "Unknown",
                RegistrationDate = user.RegistrationDate ?? DateTime.MinValue,
                LastLoginDate = user.LastLoginDate,
                IsActive = user.IsActive ?? false,

                // Nếu DTO có PasswordHash (không nên lộ ra ngoài API):
                // PasswordHash = user.PasswordHash
            };
        }

    }
}
