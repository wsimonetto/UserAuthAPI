using UserAuthAPI.Models;

namespace UserAuthAPI.Repositories
{
    public interface IUserRepository
    {
        Task<UserModel> CreateUserAsync(UserModel user);
        Task<UserModel> GetUserByIdAsync(string id);
        Task<UserModel> GetUserByEmailAsync(string email);
        Task<bool> UpdateUserAsync(UserModel user);
        Task<bool> DeleteUserAsync(string id);

    }
}
