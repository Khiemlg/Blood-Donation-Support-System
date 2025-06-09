using BloodDonation_System.Model.DTO.Auth;
using BloodDonation_System.Model.DTO.User;

namespace BloodDonation_System.Service.Interface
{
    public interface IAuthService
    {
        Task<UserDto> CreateUserByAdminAsync(CreateUserByAdminDto dto);
        Task<TokenDto> LoginAsync(LoginDto loginDto);
        Task<TokenDto> RegisterAsync(RegisterDto registerDto);
        Task SendOtpAsync(string email); // <- thêm mới
    }
}
