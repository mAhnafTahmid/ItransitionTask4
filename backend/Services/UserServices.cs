using backend.Models;
using backend.Contexts;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class UserService(MyDbContext context) : IUserService
{
    private readonly MyDbContext _context = context;

    public async Task<UserModel?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<bool> DeleteUsersAsync(List<int> userIds)
    {
        var usersToDelete = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();

        if (usersToDelete.Count == 0)
        {
            return false;
        }

        _context.Users.RemoveRange(usersToDelete);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateUserStatusAsync(List<int> userIds, string status)
    {
        var usersToUpdate = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();

        if (usersToUpdate.Count == 0)
        {
            return false;
        }

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
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.Message);
            return false;
        }
    }


}
