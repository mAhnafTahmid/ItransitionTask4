using backend.Models;

namespace backend.Services;

public interface IUserService
{
    Task<UserModel?> GetUserByIdAsync(int id);
    Task<bool> DeleteUsersAsync(List<int> userIds);
    Task<bool> UpdateUserStatusAsync(List<int> userIds, string status);
    Task<List<UserModel>> GetAllUsersAsync();
    Task<UserModel?> GetUserByEmailAsync(string email);
    Task<bool> UpdateUserAsync(UserModel user);

}