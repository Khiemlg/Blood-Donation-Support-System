using System.ComponentModel.DataAnnotations;

namespace Blood_Donation_System.BusinessLogic.MyModels.DTO
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Tên người dùng không được để trống.")]
        [RegularExpression(@"^[^\d]+$", ErrorMessage = "Tên người dùng không được chứa chữ số.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")] 
        public string Password { get; set; }
    }
}