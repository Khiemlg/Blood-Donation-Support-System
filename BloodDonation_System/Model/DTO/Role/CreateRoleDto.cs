using System.ComponentModel.DataAnnotations;

namespace BloodDonation_System.Model.DTO.Role
{
    public class CreateRoleDto
    {
        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = null!;

        [StringLength(255)]
        public string? Description { get; set; }
    }
}
