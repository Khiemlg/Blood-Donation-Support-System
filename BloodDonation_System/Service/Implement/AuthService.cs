using BloodDonation_System.Data;
using BloodDonation_System.Model.DTO.Auth;
using BloodDonation_System.Model.DTO.User;
using BloodDonation_System.Model.Enties;
using BloodDonation_System.Service.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BloodDonation_System.Service.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly DButils _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(DButils context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<UserDto> CreateUserByAdminAsync(CreateUserByAdminDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                throw new Exception("Username đã tồn tại.");

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("Email đã tồn tại.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash);

            var user = new User
            {
                UserId = GenerateCustomUserId(),
                Username = dto.Username,
                PasswordHash = hashedPassword,
                Email = dto.Email,
                RoleId = dto.RoleId,
                RegistrationDate = DateTime.UtcNow,
                IsActive = dto.IsActive ?? true
            };

            _context.Users.Add(user);
            await SaveChangesWithErrorHandling();

            return new UserDto
            {
             //  UserID = user.UserId,
               // UserName = user.Username,
                Email = user.Email,
                RoleName = (await _context.Roles.FindAsync(user.RoleId))?.RoleName ?? "Unknown",
                CreatedAt = user.RegistrationDate ?? DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users.Include(u => u.Role)
                                           .FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            //by Long
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
                throw new Exception("Email hoặc mật khẩu không đúng.");
            if(user.IsActive == false)
            {
                throw new Exception("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
            }
        
            user.LastLoginDate = DateTime.UtcNow;
            await SaveChangesWithErrorHandling();

            string token = GenerateJwtToken(user);
            return new TokenDto { Token = token };
        }

        public async Task<TokenDto> RegisterAsync(RegisterDto dto)
        {
            var otp = await _context.OtpCodes
                .FirstOrDefaultAsync(x =>
                    x.Email == dto.Email &&
                    x.Code == dto.OtpCode &&
                    !x.IsUsed &&
                    x.ExpiredAt > DateTime.UtcNow);

            if (otp == null)
                throw new Exception("Mã OTP không hợp lệ hoặc đã hết hạn.");

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("Email đã được sử dụng.");

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                throw new Exception("Username đã tồn tại.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                UserId = GenerateCustomUserId(),
                Username = dto.Username,
                PasswordHash = hashedPassword,
                Email = dto.Email,
                RoleId = 1, // default: Member
                RegistrationDate = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            otp.IsUsed = true;

            await SaveChangesWithErrorHandling();

            var token = GenerateJwtToken(user);
            return new TokenDto { Token = token };
        }

        public async Task SendOtpAsync(string email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
                throw new Exception("Email đã tồn tại.");

            var code = new Random().Next(100000, 999999).ToString();

            var otp = new OtpCode
            {
                Email = email,
                Code = code,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            _context.OtpCodes.Add(otp);
            await SaveChangesWithErrorHandling();

            string subject = "Your OTP Code for Blood Donation System";
            string body = $"""
                <h3>Xin chào 👋</h3>
                <p>Mã OTP của bạn là: <strong>{code}</strong></p>
                <p>Mã này sẽ hết hạn sau 5 phút.</p>
            """;

            await _emailService.SendEmailAsync(email, subject, body);
        }

        // -------------------- Utility Methods --------------------

        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSecret = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "1440");

            //var claims = new[]
            //{
            //    new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
            //    new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            //    new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User"),
            //    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            //};

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
                new Claim("user_id", user.UserId), // ✅ thêm dòng này
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User"),
                new Claim("isActive", (bool)user.IsActive ? "true" : "false"),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task SaveChangesWithErrorHandling()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Không thể lưu dữ liệu: " + (ex.InnerException?.Message ?? ex.Message));
            }
        }

        private string GenerateCustomUserId()
        {
            string prefix = "USER_";
            string suffix;
            do
            {
                suffix = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            }
            while (_context.Users.Any(u => u.UserId == prefix + suffix));

            return prefix + suffix;
        }
    }
}
