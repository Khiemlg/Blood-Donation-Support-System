using System.ComponentModel.DataAnnotations;

namespace BloodDonation_System.Model.DTO.Auth
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}