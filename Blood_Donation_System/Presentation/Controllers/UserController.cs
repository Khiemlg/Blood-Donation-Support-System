using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Blood_Donation_System.BusinessLogic.MyModels;
using Blood_Donation_System.BusinessLogic.MyModels.LoginRequest;
using Blood_Donation_System.DataAccess;

namespace Blood_Donation_System.Presentation.Controllers
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
                [Route("User/List")]
                public async Task<ActionResult> Read()
                {
                    return Ok(new { data = await connect.Users.ToListAsync() });
                }
                
                [HttpPost]
                [Route("User/Delete")]
                public async Task<ActionResult> Delete(int id )
                {
                 var user = await connect.Users.FirstOrDefaultAsync(x => x.UserId.Equals(id));
                 if(user == null)
                 {
                     return BadRequest("user not found");
                 }
                 
                 connect.Update(user.IsActive = false);
                 await connect.SaveChangesAsync();
                  return Ok("uses is inactive");
                }


                [HttpPost]
                [Route("User/Delete_2")]
                public async Task<ActionResult> Delete2(int id )
                {
                var user = await connect.Users.FirstOrDefaultAsync(x => x.UserId.Equals(id));
                connect.Remove(user);
                await connect.SaveChangesAsync();
                if(user == null)
                {
                     return BadRequest("user not found");
                }
                 
                     return Ok(" delete successfull "+ user);
                }
         

                [HttpPost]
                [Route("User/Insert")]
                public async Task<ActionResult> insert(string Username, int RoleID, string Email, string PhoneNumber, string PasswordHash)
                {
                    var existingUser = await connect.Users.FirstOrDefaultAsync(x => x.Email == Email);
                    if (existingUser != null)
                    {
                        BadRequest("Email already exist");
                    }
                    if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(PasswordHash) || string.IsNullOrWhiteSpace(PhoneNumber))
                    {
                        return BadRequest("Tên người dùng, Email và Mật khẩu không được để trống.");
                    }
                    if (Regex.IsMatch(Username, @"\d")) 
                    {
                        return BadRequest("user name cannot use digit");
                    }
                    
                    if(Regex.IsMatch(PhoneNumber, @"^0\d{9}$"))
                    {
                         return BadRequest("The phone number is invalid. Please enter a number starting with 0 and consisting of 10 digits.");
                    }

                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(PasswordHash);
                    User user = new User();
                    user.Username = Username;
                    user.RoleId = RoleID;
                    user.Email = Email;
                    user.PhoneNumber = PhoneNumber;
                    user.PasswordHash = hashedPassword;
                    user.IsActive = true;
                    connect.Users.Add(user);
                    await connect.SaveChangesAsync();
                    return Ok(new { data = user });
                }


                [HttpPost]
                [Route("User/Register")]
                public async Task<ActionResult> Register(string Username, string Email, string PhoneNumber, string PasswordHash)
                {
                    var existingUser = await connect.Users.FirstOrDefaultAsync(x => x.Email == Email);
                    if (existingUser != null)
                    {
                        BadRequest("Email already exist");
                    }
                     if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(PasswordHash) || string.IsNullOrWhiteSpace(PhoneNumber))
                    {
                        return BadRequest("Tên người dùng, Email và Mật khẩu không được để trống.");
                    }
                    if (Regex.IsMatch(Username, @"\d")) 
                    {
                        return BadRequest("user name cannot use digit");
                    }
                    
                    if(Regex.IsMatch(PhoneNumber, @"^0\d{9}$"))
                    {
                         return BadRequest("The phone number is invalid. Please enter a number starting with 0 and consisting of 10 digits.");
                    }

                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(PasswordHash);
                    User user = new User();
                    user.Username = Username;
                    user.RoleId = 3;
                    user.Email = Email;
                    user.PhoneNumber = PhoneNumber;
                    user.PasswordHash = hashedPassword;
                    user.IsActive = true;
                    connect.Users.Add(user);
                    await connect.SaveChangesAsync();
                    return Ok(new { data = user });
                }


                [HttpPost]
                [Route("User/Update")]
                public async Task<ActionResult> Update(String id, string Username, int RoleID, string Email, string PhoneNumber, string PasswordHash)
                {
                    var userToUpdate = await connect.Users.FirstOrDefaultAsync(x => x.UserId.Equals( id));
                    if (userToUpdate == null)
                    {
                        BadRequest("user not found");
                    }
                     if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(PasswordHash) || string.IsNullOrWhiteSpace(PhoneNumber))
                    {
                        return BadRequest("Tên người dùng, Email và Mật khẩu không được để trống.");
                    }
                    if (Regex.IsMatch(Username, @"\d")) 
                    {
                        return BadRequest("user name cannot use digit");
                    }
                    
                    if(Regex.IsMatch(PhoneNumber, @"^0\d{9}$"))
                    {
                         return BadRequest("The phone number is invalid. Please enter a number starting with 0 and consisting of 10 digits.");
                    }


                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(PasswordHash);
                    User user = new User();

                    userToUpdate.Username = Username;
                    userToUpdate.RoleId = RoleID;
                    userToUpdate.Email = Email;
                    userToUpdate.PhoneNumber = PhoneNumber;
                    userToUpdate.PasswordHash = hashedPassword;

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
                    var Role = connect.Roles.FirstOrDefault(x => x.RoleId == user.RoleId);
                    if(Role == null)
                    {
                    return Unauthorized(new { message = "User role not found" });
                    }
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email), // Hoặc user.Username
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.Role, Role.RoleName) 
                    };

                var authSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));

                var token = new JwtSecurityToken(
                   issuer: Configuration["Jwt:Issuer"],
                   audience: Configuration["Jwt:Audience"],
                   expires: DateTime.Now.AddHours(3), 
                   claims: authClaims,
                   signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                 );

             
                 return Ok(new { message = "Đăng nhập thành công!", user_id = user.UserId, email = user.Email ,
                     token = new JwtSecurityTokenHandler().WriteToken(token),
                     expiration = token.ValidTo
                 });
                }


   
       

    }

}