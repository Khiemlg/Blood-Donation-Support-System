using BloodDonation_System.Data;
using BloodDonation_System.Model.DTO.User;
using BloodDonation_System.Model.Enties;
using DrugUsePreventionAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DrugUsePreventionAPI.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly DButils _context;

        public UserService(DButils context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();

            return users.Select(user => new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                PasswordHash = user.PasswordHash,
                Email = user.Email,
                RoleId = user.RoleId,
                RegistrationDate = user.RegistrationDate ?? DateTime.UtcNow,
                LastLoginDate = user.LastLoginDate,
                IsActive = user.IsActive ?? true
            });
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            var user = await _context.Users.FindAsync(id.ToString());
            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                PasswordHash = user.PasswordHash,
                Email = user.Email,
                RoleId = user.RoleId,
                RegistrationDate = user.RegistrationDate ?? DateTime.UtcNow,
                LastLoginDate = user.LastLoginDate,
                IsActive = user.IsActive ?? true
            };
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
            var newUser = new User
            {
                UserId = "USER_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                Username = createUserDto.Username,
                PasswordHash = hashedPassword,
                Email = createUserDto.Email,
                RoleId = createUserDto.RoleId, // Default to normal user
                RegistrationDate = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserId = newUser.UserId,
                Username = newUser.Username,
                PasswordHash = newUser.PasswordHash,
                Email = newUser.Email,
                RoleId = newUser.RoleId,
                RegistrationDate = newUser.RegistrationDate.Value,
                LastLoginDate = newUser.LastLoginDate,
                IsActive = newUser.IsActive ?? true
            };
        }

        public async Task<UserDto> UpdateUserAsync(string id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id.ToString());
            if (user == null) return null;

            user.Username = updateUserDto.Username ?? user.Username;
            user.Email = updateUserDto.Email ?? user.Email;
            user.RoleId = updateUserDto.RoleId ?? user.RoleId;
            user.IsActive = updateUserDto.IsActive ?? user.IsActive;
            user.LastLoginDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                PasswordHash = user.PasswordHash,
                Email = user.Email,
                RoleId = user.RoleId,
                RegistrationDate = user.RegistrationDate ?? DateTime.UtcNow,
                LastLoginDate = user.LastLoginDate,
                IsActive = user.IsActive ?? true
            };
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _context.Users.FindAsync(id.ToString());
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserDto> UpdateUserRoleAsync(string id, string roleName, string callerRole)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id.ToString());
            if (user == null) return null;

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
            if (role == null) return null;

            user.RoleId = role.RoleId;
            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                PasswordHash = user.PasswordHash,
                Email = user.Email,
                RoleId = user.RoleId,
                RegistrationDate = user.RegistrationDate ?? DateTime.UtcNow,
                LastLoginDate = user.LastLoginDate,
                IsActive = user.IsActive ?? true
            };
        }
    }
}
