using backend.Models;
using backend.Contexts;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class UserService(MyDbContext context) : IUserService
{
    private readonly MyDbContext _context = context;

    public async Task<UserModel?> GetUserByIdAsync(int id)
    {
        // Fetch the user with the specified ID
        return await _context.Users.FindAsync(id);
    }

    public async Task<bool> DeleteUsersAsync(List<int> userIds)
    {
        // Fetch users to delete
        var usersToDelete = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();

        if (usersToDelete.Count == 0)
        {
            return false; // No users found to delete
        }

        _context.Users.RemoveRange(usersToDelete);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateUserStatusAsync(List<int> userIds, string status)
    {
        // Fetch users to update
        var usersToUpdate = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();

        if (usersToUpdate.Count == 0)
        {
            return false; // No users found to update
        }

        // Update the status
        foreach (var user in usersToUpdate)
        {
            user.Status = status;
        }

        await _context.SaveChangesAsync();
        return true;
    }



    public async Task<List<UserModel>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<UserModel?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
                             .SingleOrDefaultAsync(user => user.Email == email);
    }

    public async Task<bool> UpdateUserAsync(UserModel user)
    {
        try
        {
            _context.Users.Update(user); // Assuming `_dbContext.Users` is your DbSet<UserModel>
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception for debugging purposes
            Console.WriteLine(ex.Message);
            return false;
        }
    }


}
