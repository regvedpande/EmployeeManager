using AssignmentEmployee.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer; // 👈 NEW IMPORT
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // 👈 NEW IMPORT
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Database Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Add Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 👇 THIS LINE STOPS THE CRASH (Error 500)
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. 🔐 JWT AUTHENTICATION SETUP (The Fix)
var jwtKey = "ThisIsMySuperSecretKey1234567890!!"; // ⚠️ Must be at least 32 chars
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
// ---------------------------------------------------------

// 4. CORS (Allow Frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll"); // 👈 Must be before Auth

app.UseAuthentication(); // 👈 Must be here
app.UseAuthorization();

app.MapControllers();

app.Run();