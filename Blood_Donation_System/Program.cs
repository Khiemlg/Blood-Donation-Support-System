using System.Text;
using Blood_Donation_System.MyModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
string cnn = builder.Configuration.GetConnectionString("cnn") ?? throw new InvalidOperationException("Connection string 'cnn' not found.");
builder.Services.AddDbContext<DButils>(options => options.UseSqlServer(cnn));

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


builder.Services.AddAuthorization();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "BloodDonationSystem");
    });
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
