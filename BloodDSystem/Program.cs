using BloodDSystem.MyModels;
//using BloodSystem.MyModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // Assuming AddOpenApi is a valid extension for Swagger

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


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