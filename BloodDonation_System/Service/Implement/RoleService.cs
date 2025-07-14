using BloodDonation_System.Data;
using BloodDonation_System.Model.DTO.Role;
using BloodDonation_System.Model.Enties;
using BloodDonation_System.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation_System.Service.Implement
{
    public class RoleService : IRoleService
    {
        private readonly DButils _context;

        public RoleService(DButils context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            return await _context.Roles
                .Select(r => new RoleDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    Description = r.Description
                })
                .ToListAsync();
        }

        public async Task<RoleDto?> GetByIdAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return null;

            return new RoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Description = role.Description
            };
        }

        public async Task<RoleDto?> CreateAsync(CreateRoleDto dto)
        {
            if (await _context.Roles.AnyAsync(r => r.RoleName == dto.RoleName))
                return null;

            var role = new Role
            {
                RoleName = dto.RoleName,
                Description = dto.Description
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return new RoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Description = role.Description
            };
        }

        public async Task<RoleDto?> UpdateAsync(int id, CreateRoleDto dto)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return null;

            if (role.RoleName != dto.RoleName && await _context.Roles.AnyAsync(r => r.RoleName == dto.RoleName))
                return null; 

            role.RoleName = dto.RoleName;
            role.Description = dto.Description;

            await _context.SaveChangesAsync();

            return new RoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Description = role.Description
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return false;

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
