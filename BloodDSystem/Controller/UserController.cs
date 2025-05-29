using BloodSystem.MyModels.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloodDSystem.MyModels;
using NETCore.MailKit.Core;
namespace BloodSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly DButils connect;
        private readonly IConfiguration Configuration;
        private readonly IEmailService EmailService;
        public UserController(DButils C_in, IConfiguration configuration, IEmailService emailservice)
        {
            connect = C_in;
            Configuration = configuration;
            EmailService = emailservice;
        }

        [HttpGet]
        [Route("/User/List")]
        public async Task<ActionResult> Read()
        {
            return Ok(new { data = await connect.Users.ToListAsync() });
        }

        [HttpPost]
        [Route("User/Insert")]
        public async Task<ActionResult> insert(String FullName, int RoleID, String Email, String PhoneNumber, String PasswordHash)
        {
            var existingUser = await connect.Users.FirstOrDefaultAsync(x => x.Email == Email);
            if (existingUser != null)
            {
                BadRequest("Email already exist");
            }

            User user = new User();
            user.FullName = FullName;
            user.RoleId = RoleID;
            user.Email = Email;
            user.PhoneNumber = PhoneNumber;
            user.PasswordHash = PasswordHash;
            user.IsActive = true;
            connect.Users.Add(user);
            await connect.SaveChangesAsync();
            return Ok(new { data = user });
        }

        [HttpPost]
        [Route("User/Update")]
        public async Task<ActionResult> Update(int id, String FullName, int RoleID, String Email, String PhoneNumber, String PasswordHash)
        {
            var userToUpdate = await connect.Users.FirstOrDefaultAsync(x => x.UserId == id);
            if (userToUpdate == null)
            {
                BadRequest("user not found");
            }


            User user = new User();

            userToUpdate.FullName = FullName;
            userToUpdate.RoleId = RoleID;
            userToUpdate.Email = Email;
            userToUpdate.PhoneNumber = PhoneNumber;
            userToUpdate.PasswordHash = PasswordHash;

            connect.Users.Add(user);
            await connect.SaveChangesAsync();
            return Ok(new { data = user });
        }

        [HttpPost]
        [Route("User/Login")]
        public async Task<ActionResult> Login([FromBody] UserLoginRequest loginrequest)
        {
            if (loginrequest is null)
            {
                throw new ArgumentNullException(nameof(loginrequest));
            }

            var user = await connect.Users.FirstOrDefaultAsync(x => x.Email == loginrequest.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Email invalid  or  wrong login password." });
            }
            if (!BCrypt.Net.BCrypt.Verify(loginrequest.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Email invalid  or  wrong login password.." });
            }
            if (!user.IsEmailVerified)
            {
                return Unauthorized(new { message = "Email not verified." });
            }
            return Ok(new { message = "Đăng nhập thành công!", user_id = user.UserId, email = user.Email });
        }
    }
}