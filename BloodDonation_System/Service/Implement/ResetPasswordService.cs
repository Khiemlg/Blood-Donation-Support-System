using BloodDonation_System.Data;
using BloodDonation_System.Model.DTO.Auth;
using BloodDonation_System.Model.Enties;
using BloodDonation_System.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation_System.Service.Implement
{
    public class ResetPasswordService : IResetPasswordService
    {
        private readonly DButils _context;
        private readonly IEmailService _emailService;

        public ResetPasswordService(DButils context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<bool> SendOtpToEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            var otp = new Random().Next(100000, 999999).ToString();
            var expiredAt = DateTime.UtcNow.AddMinutes(10);

            _context.OtpCodes.Add(new OtpCode
            {
                Email = email,
                Code = otp,
                ExpiredAt = expiredAt,
                IsUsed = false
            });

            await _context.SaveChangesAsync();
            await _emailService.SendEmailAsync(email, "OTP for Reset Password", $"Your OTP code is: {otp}");
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return false;

            var otpRecord = await _context.OtpCodes
                .Where(x => x.Email == dto.Email && x.Code == dto.Otp && !x.IsUsed && x.ExpiredAt > DateTime.UtcNow)
                .OrderByDescending(x => x.ExpiredAt)
                .FirstOrDefaultAsync();

            if (otpRecord == null) return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            otpRecord.IsUsed = true;

            await _context.SaveChangesAsync();
            return true;
        }
    }

}
