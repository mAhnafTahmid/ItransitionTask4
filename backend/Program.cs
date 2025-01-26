using backend.Contexts;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using backend.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;


var builder = WebApplication.CreateBuilder(args);


// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5189", "http://localhost:8080") // Add your frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Load environment variables from .env file
Env.Load();

builder.Services.AddControllers();

// Get environment-specific database URL
var environment = builder.Environment.EnvironmentName;
var dbUrl = environment == "Production"
    ? Environment.GetEnvironmentVariable("db_internal_url")  // Use Internal URL in production
    : Environment.GetEnvironmentVariable("db_external_url"); // Use External URL in development

string? dbHost = Environment.GetEnvironmentVariable("db_hostname");
string? dbName = Environment.GetEnvironmentVariable("db_name");
string? dbUser = Environment.GetEnvironmentVariable("db_user");
string? dbPassword = Environment.GetEnvironmentVariable("db_password");
var dbPort = 5432;

if (string.IsNullOrEmpty(dbUrl))
{
    Console.WriteLine("Database URL is not set.");
}
else
{
    Console.WriteLine($"Using database URL: {dbUrl}");
}

var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Trust Server Certificate=true;";

// Register UserService
builder.Services.AddScoped<IUserService, UserService>();

// Add DbContext with connection string
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseNpgsql(connectionString));

// JWT Authentication Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // We are only validating the signature and lifetime of the token
            ValidateIssuer = false,        // Skip issuer validation
            ValidateAudience = false,      // Skip audience validation
            ValidateLifetime = true,       // Ensure the token has not expired
            ValidateIssuerSigningKey = true, // Ensure the signing key is valid

            // Here, we use the secret key stored in the environment
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SecretKey"))),
        };
    });

// Add Authorization services
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

// Use Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Configure routing
app.MapControllers();

// Test endpoint
app.MapGet("/", () => "Hello World!");

// Run the application
app.Run();
