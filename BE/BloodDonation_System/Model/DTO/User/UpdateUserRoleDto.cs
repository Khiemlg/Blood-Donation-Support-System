using System.ComponentModel.DataAnnotations;

namespace BloodDonation_System.Models.DTOs.User
{
    public class UpdateUserRoleDto
    {
        [Required]
        public string RoleName { get; set; }
    }
}
