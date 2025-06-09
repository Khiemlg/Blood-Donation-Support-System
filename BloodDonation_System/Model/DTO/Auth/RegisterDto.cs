namespace BloodDonation_System.Model.DTO.Auth
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }   // Đổi tên cho đúng bản chất
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }

}
