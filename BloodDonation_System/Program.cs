using BloodDonation_System.Data;
using BloodDonation_System.Service.Interface;
using BloodDonation_System.Service.Implementation;
using BloodDonation_System.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BloodDonation_System.Utilities;
using BloodDonation_System.Service.Implement;

var builder = WebApplication.CreateBuilder(args);

// ✅ Cấu hình DbContext
string cnn = builder.Configuration.GetConnectionString("cnn")
    ?? throw new InvalidOperationException("Connection string 'cnn' not found.");
builder.Services.AddDbContext<DButils>(options => options.UseSqlServer(cnn));

// ✅ Cấu hình HttpClient (cho Geocoding hoặc gọi API ngoài)
builder.Services.AddHttpClient<GeocodingService>();

// ✅ Redis Cache (nếu bạn dùng để lưu OTP hoặc session)
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "OTP_";
});



builder.Services.AddScoped<IEmergencyRequestService, EmergencyRequestService>();




// ✅ CORS cho frontend (React chạy port 3000)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",  // khiem
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

// ✅ Đăng ký các service
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>(); // ✅ Email gửi OTP
builder.Services.AddScoped<IDonationRequestService, DonationRequestService>();
builder.Services.AddScoped<IDonationHistoryService, DonationHistoryService>();
// ✅ Swagger cấu hình chuẩn
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Cấu hình JWT Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer not found."),
            ValidAudience = builder.Configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT Audience not found."),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"]
                    ?? throw new InvalidOperationException("JWT Key not found.")
            ))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ✅ Swagger hiển thị root nếu là môi trường DEV
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BloodDonationSystem");
        options.RoutePrefix = ""; // http://localhost:5000/
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
