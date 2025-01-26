using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Contexts;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace backend.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController(MyDbContext context, IConfiguration configuration) : ControllerBase
{
    private readonly MyDbContext _context = context;
    private readonly IConfiguration _configuration = configuration;

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Check if user exists with the provided email
        var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

        if (user == null)
        {
            return Unauthorized(new { Error = "Invalid email." });
        }
        if (user.Password != request.Password)
        {
            return Unauthorized(new { Error = "Invalid password." });
        }

        // Generate tokens
        var authToken = GenerateJwtToken(user);

        // Update the user's LastSeen field and save changes
        user.LastSeen = DateTime.UtcNow;
        _context.SaveChanges();

        // Return tokens in response
        return Ok(new
        {
            AuthToken = authToken,
        });
    }

    private string GenerateJwtToken(UserModel user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtKey = Environment.GetEnvironmentVariable("SecretKey"); // Get the key from an environment variable

        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT key is not configured.");
        }

        var key = Encoding.UTF8.GetBytes(jwtKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(15), // Auth token validity
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

}

// Request model for login
public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
