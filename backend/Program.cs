using backend.Contexts;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using backend.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "http://localhost:5189",
            "http://localhost:8080",
            "https://itransitiontask4-frontend.onrender.com"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});


Env.Load();

builder.Services.AddControllers();

var environment = builder.Environment.EnvironmentName;
var dbUrl = environment == "Production"
    ? Environment.GetEnvironmentVariable("db_internal_url")
    : Environment.GetEnvironmentVariable("db_external_url");

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

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {

            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SecretKey"))),
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
