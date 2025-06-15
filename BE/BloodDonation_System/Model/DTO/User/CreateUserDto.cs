using System;
using System.ComponentModel.DataAnnotations;

namespace BloodDonation_System.Model.DTO.User
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(255, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;
        public int RoleId { get; set; }  
        public bool IsActive { get; set; } = true;
    }
}
