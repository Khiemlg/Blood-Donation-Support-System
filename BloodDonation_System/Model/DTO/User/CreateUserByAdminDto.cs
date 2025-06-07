namespace BloodDonation_System.Model.DTO.User
{
    public class CreateUserByAdminDto
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public bool? IsActive { get; set; }
    }
}
