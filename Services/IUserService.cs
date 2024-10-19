using UserAuthAPI.Models;

namespace UserAuthAPI.Services
{
    public interface IUserService
    {
        Task<UserModel> CreateUserAsync(UserModel user);
        Task<UserModel> GetUserByIdAsync(string id);
        Task<UserModel> GetUserByEmailAsync(string email);
        Task<bool> UpdateUserAsync(UserModel user);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> SendPasswordRecoveryInstructionsAsync(string email); // Este é o método correto para recuperação de senha
        Task<UserModel> LoginAsync(string email, string password);
    }
}
