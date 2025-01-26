using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Contexts;
using Microsoft.EntityFrameworkCore;
using backend.Services;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(MyDbContext context, IUserService userService) : ControllerBase
{
    private readonly MyDbContext _context = context;
    private readonly IUserService? _userService = userService;

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        var newUser = new UserModel
        {
            Name = request.Name,
            Email = request.Email,
            LastSeen = DateTime.UtcNow,
            Password = request.Password,
            Status = "active"
        };

        try
        {
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully!" });
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("duplicate key") == true)
            {
                return Conflict(new { Error = "A user with this email already exists." });
            }

            return StatusCode(500, new { Error = "An error occurred while registering the user." });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(int id, [FromBody] ListInRequest request)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { Error = "User not found." });
        }

        if (user.Status != "active")
        {
            return BadRequest(new { Error = "Cannot delete user/s because you are blocked." });
        }

        var success = await _userService.DeleteUsersAsync(request.UserIds);
        if (success)
        {
            return Ok("Users successfully deleted.");
        }

        return BadRequest(new { Error = "Error deleting users." });
    }


    [HttpPatch("block/{id}")]
    [Authorize]
    public async Task<IActionResult> BlockUser(int id, [FromBody] ListInRequest request)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { Error = "User not found." });
        }

        if (user.Status != "active")
        {
            return BadRequest(new { Error = "Cannot block user/s because you are blocked." });
        }

        var success = await _userService.UpdateUserStatusAsync(request.UserIds, "blocked");
        if (success)
        {
            return Ok("Users successfully blocked.");
        }

        return BadRequest(new { Error = "Error blocking users." });
    }

    [HttpPatch("activate/{id}")]
    [Authorize]
    public async Task<IActionResult> ActivateUser(int id, [FromBody] ListInRequest request)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { Error = "User not found." });
        }

        if (user.Status != "active")
        {
            return BadRequest(new { Error = "Cannot activate user/s because you are blocked." });
        }

        var success = await _userService.UpdateUserStatusAsync(request.UserIds, "active");
        if (success)
        {
            return Ok("Users successfully activated.");
        }

        return BadRequest(new { Error = "Error activating users." });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        if (users != null && users.Any())
        {
            return Ok(users);
        }

        return NotFound(new { Error = "No users found." });
    }

    [HttpGet("email/{email}")]
    [Authorize]
    public async Task<IActionResult> GetUser(string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        if (user == null)
        {
            return NotFound(new { Error = "User not found." });
        }
        user.LastSeen = DateTime.UtcNow;
        var updateSuccess = await _userService.UpdateUserAsync(user);
        if (!updateSuccess)
        {
            return StatusCode(500, new { Error = "Failed to update user's last seen time." });
        }
        return Ok(user);
    }

}

public class RegisterUserRequest
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class ListInRequest
{
    public List<int>? UserIds { get; set; }
}

