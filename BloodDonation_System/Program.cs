using BloodDonation_System.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// ✅ Sửa đúng Swagger
builder.Services.AddEndpointsApiExplorer(); // Cho Swagger hoạt động đúng
builder.Services.AddSwaggerGen();          // Thêm generator

// ✅ Cấu hình DB
string cnn = builder.Configuration.GetConnectionString("cnn")
    ?? throw new InvalidOperationException("Connection string 'cnn' not found.");
builder.Services.AddDbContext<DButils>(options => options.UseSqlServer(cnn));

// ✅ JWT
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
                ?? throw new InvalidOperationException("JWT Issuer not found in configuration."),
            ValidAudience = builder.Configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT Audience not found in configuration."),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"]
                    ?? throw new InvalidOperationException("JWT Key not found in configuration.")
            ))
        };
    });

builder.Services.AddAuthorization();

// ✅ MailKit cấu hình
builder.Services.AddMailKit(optionBuilder =>
{
    optionBuilder.UseMailKit(new MailKitOptions()
    {
        Server = builder.Configuration["Email:SmtpHost"],
        Port = int.Parse(builder.Configuration["Email:SmtpPort"] ?? "587"),
        SenderName = builder.Configuration["Email:SenderName"] ?? "Your App",
        SenderEmail = builder.Configuration["Email:SenderEmail"] ?? "no-reply@example.com",
        Account = builder.Configuration["Email:SmtpUser"],
        Password = builder.Configuration["Email:SmtpPass"],
        Security = true
    });
});

// ✅ Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "OTP_";
});

// ✅ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendOrigin",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

var app = builder.Build();

// ✅ Swagger UI hiển thị ở root nếu là Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BloodDonationSystem");
        options.RoutePrefix = ""; // Truy cập swagger ở: http://localhost:5000/
    });
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowFrontendOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
