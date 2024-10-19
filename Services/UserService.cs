using MongoDB.Driver;
using UserAuthAPI.Models;
using System.Threading.Tasks;
using UserAuthAPI.Data;
using UserAuthAPI.Repositories;

namespace UserAuthAPI.Services
{
    public class UserService : IUserService
    {
        private readonly MongoDBContext _context;
        private readonly IPasswordService _passwordService;

        public UserService(MongoDBContext context, IPasswordService passwordService )
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<UserModel> CreateUserAsync(UserModel user)
        {

            var existingUser = await GetUserByEmailAsync(email: user.Email);
            if (existingUser != null)
            {
                throw new Exception("Um usuário com esse e-mail já está cadastrado.");
            }

            user.PasswordHash = _passwordService.HashPassword(user.PasswordHash);
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
            // Não altere a senha aqui, a menos que você queira implementar um método específico para isso
            var result = await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var result = await _context.Users.DeleteOneAsync(user => user.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<bool> SendPasswordRecoveryInstructionsAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
            {
                return false; // Email não encontrado
            }

            // Lógica para enviar e-mail de recuperação (implementação não incluída)
            return true; // Instruções enviadas com sucesso
        }
        public async Task<UserModel> LoginAsync(string email, string password)
        {
            var usuario = await GetUserByEmailAsync(email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
            {
                // Retornar null em vez de lançar exceção
                return null;
            }
            return usuario;
        }


    }
}
