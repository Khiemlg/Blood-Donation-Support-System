using BloodDonation_System.Model.DTO.Auth;

namespace BloodDonation_System.Service.Interface
{
    public interface IResetPasswordService
    {
        Task<bool> SendOtpToEmailAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    }

}
