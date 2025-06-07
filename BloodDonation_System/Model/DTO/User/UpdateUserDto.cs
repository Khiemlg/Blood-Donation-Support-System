using System;
using System.ComponentModel.DataAnnotations;

namespace BloodDonation_System.Model.DTO.User
{
    public class UpdateUserDto
    {
     
        [StringLength(50)]
        public string? Username { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? FullName { get; set; }

        public int? RoleId { get; set; }

        public bool? IsActive { get; set; }
    }
}
