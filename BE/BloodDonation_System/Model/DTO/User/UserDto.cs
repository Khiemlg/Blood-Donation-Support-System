namespace BloodDonation_System.Model.DTO.User
{
    public class UserDto
    {
        public string UserId { get; set; } = null!;
        public string UserID { get; internal set; }
        public string Username { get; set; } = null!;
        public string UserName { get; internal set; }
        public string PasswordHash { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public DateTime? RegistrationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool? IsActive { get; set; }
        public DateTime UpdatedAt { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
    }
}
