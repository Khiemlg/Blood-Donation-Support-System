using BloodDSystem.MyModels;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NETCore.MailKit.Core;
using System.Net.NetworkInformation;
using System.Text;
using NETCore.MailKit.Infrastructure.Internal;
using NETCore.MailKit.Extensions;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // Assuming AddOpenApi is a valid extension for Swagger

// Configure DbContext
string cnn = builder.Configuration.GetConnectionString("cnn") ?? throw new InvalidOperationException("Connection string 'cnn' not found.");
builder.Services.AddDbContext<DButils>(options => options.UseSqlServer(cnn));

// Configure JWT Authentication Service
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not found in configuration."),
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not found in configuration."),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration.")
            ))
        };
    });

// Configure Authorization Service
builder.Services.AddAuthorization();
//builder.Services.AddTransient<IEmailService, EmailService>();

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
        Security = true // Adjust based on your SMTP server
    });
});
var app = builder.Build();

// Configure CORS policy - Should be early in the pipeline
app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // For Swagger UI
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "BloodDonationSystem");
    });
}

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();