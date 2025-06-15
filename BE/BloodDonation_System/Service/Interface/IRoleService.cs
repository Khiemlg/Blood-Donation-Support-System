using BloodDonation_System.Model.DTO.Role;

namespace BloodDonation_System.Service.Interface
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllAsync();
        Task<RoleDto?> GetByIdAsync(int id);
        Task<RoleDto?> CreateAsync(CreateRoleDto dto);
        Task<RoleDto?> UpdateAsync(int id, CreateRoleDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
