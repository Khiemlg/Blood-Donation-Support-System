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
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Blood_Donation_System.BusinessLogic.MyModels.DTO;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Blood_Donation_System.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly DButils connect;
        private readonly IConfiguration Configuration;
        private readonly IEmailService EmailService;
      
        private readonly IDistributedCache _cache;
        public UserController(DButils C_in, IConfiguration configuration, IEmailService emailservice, IDistributedCache cache)
        {
            connect = C_in;
            Configuration = configuration;
            EmailService = emailservice;
            _cache = cache;
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
                public async Task<ActionResult> insert(string Username, int RoleID, string Email, string PasswordHash)
                {
                    
                    var existingUser = await connect.Users.FirstOrDefaultAsync(x => x.Email == Email);
                    if (existingUser != null)
                    {
                        BadRequest("Email already exist");
                    }
                    if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(PasswordHash))
                    {
                        return BadRequest("Tên người dùng, Email và Mật khẩu không được để trống.");
                    }
                    if (Regex.IsMatch(Username, @"\d")) 
                    {
                        return BadRequest("user name cannot use digit");
                    }
                    
                 //   if(Regex.IsMatch(PhoneNumber, @"^0\d{9}$"))
                   // {
                   //      return BadRequest("The phone number is invalid. Please enter a number starting with 0 and consisting of 10 digits.");
                   // }

                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(PasswordHash);
                    User user = new User();
                    string uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(); 
                    user.UserId = "USER_" + uniqueSuffix;
                    user.Username = Username;
                    user.RoleId = RoleID;
                    user.Email = Email;
                    
                    user.PasswordHash = hashedPassword;
                    user.IsActive = true;
                    connect.Users.Add(user);
                    await connect.SaveChangesAsync();
                    return Ok(new { data = user });
                }


        /* [HttpPost]
         [Route("Register")]
         public async Task<ActionResult> Register([FromBody] UserRegisterDto model) // Nhận UserRegisterDto
         {
             if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState); // Trả về lỗi xác thực chi tiết
             }

             var existingUser = await connect.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
             if (existingUser != null)
             {
                 return Conflict("Email đã tồn tại.");
             }

             Otp o1 = new Otp();
             string otp = o1.GenerateOtp();

             var tempRegData = new UserRegistrationData()
             {
                 OtpCode = otp,
                 Username = model.Username,
                 PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password), // Hash mật khẩu trước khi lưu tạm vào cache
                 Email = model.Email // Lưu email vào đây để tiện cho bước VerifyOtp
             };

             var cacheKey = $"RegOtp_{model.Email}"; // Key cache dựa trên email
             var options = new DistributedCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromMinutes(5)); // OTP có hiệu lực trong 5 phút

             string jsonData = JsonSerializer.Serialize(tempRegData);
             await _cache.SetStringAsync(cacheKey, jsonData, options);

             try
             {
                 await EmailService.SendAsync(
                     model.Email,
                     "Mã OTP xác minh đăng ký của bạn",
                     $"Mã OTP của bạn là: <b>{otp}</b>. Mã này chỉ có giá trị trong một thời gian ngắn ({options.SlidingExpiration?.TotalMinutes} phút).",
                     isHtml: true
                 );
                 return Ok(new { message = "Mã OTP đã được gửi đến email của bạn. Vui lòng xác minh để hoàn tất đăng ký." });
             }
             catch (Exception ex)
             {
                 Console.WriteLine($"Lỗi khi gửi email OTP cho {model.Email}: {ex.Message}");
                 await _cache.RemoveAsync(cacheKey); // Xóa dữ liệu tạm nếu gửi email thất bại
                 return StatusCode(500, "Không thể gửi email OTP. Vui lòng thử lại sau.");
             }
         }


         [HttpPost]
         [Route("VerifyOtp")]
         public async Task<ActionResult> VerifyOtp(string Email, string Otp)
         {
             if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Otp))
             {
                 return BadRequest("Email và mã OTP không được để trống.");
             }

             var cacheKey = $"RegOtp_{Email}";
             string? jsonData = await _cache.GetStringAsync(cacheKey);

             if (string.IsNullOrEmpty(jsonData))
             {
                 return BadRequest("Mã OTP không hợp lệ hoặc đã hết hạn. Vui lòng đăng ký lại.");
             }

             var tempRegData = JsonSerializer.Deserialize<UserRegistrationData>(jsonData);

             // Kiểm tra null và so sánh OTP
             if (tempRegData == null || tempRegData.OtpCode != Otp)
             {
                 return BadRequest("Mã OTP không đúng. Vui lòng thử lại.");
             }

             // Kiểm tra lại email trong DB để tránh trường hợp người dùng cố gắng đăng ký lại sau khi OTP hết hạn
             var existingUser = await connect.Users.FirstOrDefaultAsync(x => x.Email == Email);
             if (existingUser != null)
             {
                 await _cache.RemoveAsync(cacheKey); // Xóa cache OTP đã hết tác dụng
                 return Conflict("Email này đã có tài khoản đăng ký. Vui lòng đăng nhập.");
             }

             User user = new User();
             string uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
             user.UserId = "USER_" + uniqueSuffix; // Tạo ID duy nhất
             user.Username = tempRegData.Username;
             user.RoleId = 3; // Mặc định role là User (3)
             user.Email = tempRegData.Email; // Lấy email từ dữ liệu tạm
             user.PasswordHash = tempRegData.PasswordHash; // Lấy password hash từ dữ liệu tạm
             user.IsActive = true;

             connect.Users.Add(user);
             await connect.SaveChangesAsync();

             await _cache.RemoveAsync(cacheKey); // Xóa OTP khỏi cache sau khi đăng ký thành công

             return Ok(new { data = user, message = "Đăng ký thành công!" });
         }




         */
        [HttpPost]
        [Route("User/Register")]
        public async Task<ActionResult> Register(string Username, string Email, string PasswordHash)
        {
            // Kiểm tra email đã tồn tại (vẫn giữ để cung cấp thông báo rõ ràng hơn trước khi vào DB)
            var existingUserByEmail = await connect.Users.FirstOrDefaultAsync(x => x.Email == Email);
            if (existingUserByEmail != null)
            {
                return BadRequest("Email already exists.");
            }

            // Kiểm tra Username đã tồn tại (vẫn giữ)
            var existingUserByUsername = await connect.Users.FirstOrDefaultAsync(x => x.Username == Username);
            if (existingUserByUsername != null)
            {
                return BadRequest("Username already exists.");
            }

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(PasswordHash))
            {
                return BadRequest("Tên người dùng, Email và Mật khẩu không được để trống.");
            }
            if (Regex.IsMatch(Username, @"\d"))
            {
                return BadRequest("User name cannot contain digits.");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(PasswordHash);
            User user = new User();

            user.UserId = Guid.NewGuid().ToString();

            user.Username = Username;
            user.RoleId = 3; // Mặc định role là Member (3)
            user.Email = Email;

            user.PasswordHash = hashedPassword;
            user.IsActive = true;

            try
            {
                connect.Users.Add(user);
                await connect.SaveChangesAsync();
                return Ok(new { data = user, message = "Đăng ký thành công!" });
            }
            catch (DbUpdateException ex)
            {
                // Kiểm tra InnerException để xác định loại lỗi SQL
                if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
                {
                    // Lỗi 2627: Violation of UNIQUE KEY constraint (cho các UNIQUE INDEX)
                    // Lỗi 2601: Violation of PRIMARY KEY constraint (cho PRIMARY KEY)
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                    {
                        // Bạn có thể phân tích sqlEx.Message để xác định cụ thể cột nào bị trùng lặp
                        // Ví dụ: "Violation of UNIQUE KEY constraint 'UQ__Users__F3DBC572EA33BF68'. Cannot insert duplicate key in object 'dbo.Users'. The duplicate key value is (testuser)."
                        if (sqlEx.Message.Contains("UQ__Users__F3DBC572EA33BF68")) // Ràng buộc Username
                        {
                            return BadRequest("Username already exists. Please choose a different username.");
                        }
                        else if (sqlEx.Message.Contains("UQ__Users__AB6E6164159AA181")) // Ràng buộc Email
                        {
                            return BadRequest("Email already exists. Please choose a different email.");
                        }
                        else
                        {
                            // Lỗi trùng lặp không xác định
                            return BadRequest("A duplicate entry was found. Please check your username or email.");
                        }
                    }
                }
                // Nếu không phải lỗi trùng lặp, ném lại ngoại lệ
                throw;
            }
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
                        return BadRequest("User name , Email và password  is not empty.");
                    }
                    if (Regex.IsMatch(Username, @"\d")) 
                    {
                        return BadRequest("user name cannot use digit");
                    }
                    
                

                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(PasswordHash);
                    User user = new User();

                    userToUpdate.Username = Username;
                    userToUpdate.RoleId = RoleID;
                    userToUpdate.Email = Email;
                    
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
                    user.RoleName = Role.RoleName;
                    var token = CreateToken(user);


                 return Ok(new { message = "Login Successfully!", user_id = user.UserId, email = user.Email ,
                     token = token.Token,
                     expiration = token.Expiration
                 });
                }





        [NonAction]
        public AuthTokenResult CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleName)
            };

            // Lấy JWT Key từ cấu hình và kiểm tra null/empty
            var jwtKey = Configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Secret Key (Jwt:Key) is not configured or is empty in appsettings.json. Please ensure 'Jwt:Key' is set correctly.");
            }

            // Chuyển đổi khóa thành byte array và kiểm tra độ dài
            byte[] keyBytes = Encoding.UTF8.GetBytes(jwtKey);
            const int requiredKeySizeBits = 512; // HS512 requires 512 bits
            if (keyBytes.Length * 8 < requiredKeySizeBits)
            {
                throw new InvalidOperationException($"JWT Secret Key (Jwt:Key) is too short. It must be at least {requiredKeySizeBits} bits ({requiredKeySizeBits / 8} bytes) long for HS512 algorithm. Current key has {keyBytes.Length * 8} bits.");
            }

            var key = new SymmetricSecurityKey(keyBytes); // Sử dụng keyBytes đã kiểm tra
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var expires = DateTime.UtcNow.AddDays(1);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: Configuration["Jwt:Issuer"],
                audience: Configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new AuthTokenResult
            {
                Token = tokenString,
                Expiration = expires
            };
        }




    }

}