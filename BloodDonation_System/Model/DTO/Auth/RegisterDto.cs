namespace BloodDonation_System.Model.DTO.Auth
{
    public class RegisterDto
    {
       //public string OtpCode { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
    }
}
