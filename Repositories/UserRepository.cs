using MongoDB.Driver;
using UserAuthAPI.Models;
using System.Threading.Tasks;
using UserAuthAPI.Data;

namespace UserAuthAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MongoDBContext _context;

        public UserRepository(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<UserModel> CreateUserAsync(UserModel user)
        {
            await _context.Users.InsertOneAsync(user);
            return user;
        }

        public async Task<UserModel> GetUserByIdAsync(string id)
        {
            return await _context.Users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        public async Task<UserModel> GetUserByEmailAsync(string email)
        {
            return await _context.Users.Find(user => user.Email == email).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateUserAsync(UserModel user)
        {
            var result = await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var result = await _context.Users.DeleteOneAsync(user => user.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<UserModel> LoginAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) // Use o BCrypt para verificar a senha
            {
                return user;
            }
            throw new UnauthorizedAccessException("Email ou senha incorretos."); // Lança uma exceção se o login falhar
        }


        public async Task<bool> RecoverPasswordAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            if (user != null)
            {
                // Lógica para enviar e-mail de recuperação (implementação não incluída)
                return true; // Retorne true se o e-mail for enviado com sucesso
            }
            return false; // Retorne false se o usuário não for encontrado
        }

    }
}
