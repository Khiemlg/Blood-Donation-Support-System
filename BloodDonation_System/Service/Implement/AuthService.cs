using BloodDonation_System.Data;
using BloodDonation_System.Model.DTO.Auth;
using BloodDonation_System.Model.DTO.User;
using BloodDonation_System.Model.Enties;
using BloodDonation_System.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

        public AuthService(DButils context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<UserDto> CreateUserByAdminAsync(CreateUserByAdminDto dto)
        {
            // Kiểm tra username hoặc email đã tồn tại chưa
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                throw new Exception("Username đã tồn tại.");

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("Email đã tồn tại.");

            var user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                Username = dto.Username,
                PasswordHash = dto.PasswordHash,
                Email = dto.Email,
                RoleId = dto.RoleId,
                RegistrationDate = DateTime.UtcNow,
                IsActive = dto.IsActive ?? true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserID = user.UserId,
                UserName = user.Username,
                Email = user.Email,
                RoleName = (await _context.Roles.FindAsync(user.RoleId))?.RoleName ?? "Unknown",
                CreatedAt = user.RegistrationDate ?? DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
                throw new Exception("Email hoặc mật khẩu không đúng.");

            // TODO: So sánh mật khẩu (bạn cần implement kiểm tra hash mật khẩu)
            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
                throw new Exception("Email hoặc mật khẩu không đúng.");

            // Cập nhật last login
            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            string token = GenerateJwtToken(user);

            return new TokenDto { Token = token };
        }

        public async Task<TokenDto> RegisterAsync(RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                throw new Exception("Email đã được sử dụng.");

            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                throw new Exception("Username đã tồn tại.");

            var user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                Username = registerDto.Username,
                PasswordHash = registerDto.PasswordHash,
                Email = registerDto.Email,
                RoleId = 2, 
                RegistrationDate = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            string token = GenerateJwtToken(user);

            return new TokenDto { Token = token };
        }

        // Hàm kiểm tra mật khẩu, ví dụ dùng BCrypt
        private bool VerifyPassword(string password, string passwordHash)
        {
            return password == passwordHash;
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSecret = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expireMinutes = int.Parse(_configuration["JwtSettings:ExpireMinutes"] ?? "1440");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User"),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
