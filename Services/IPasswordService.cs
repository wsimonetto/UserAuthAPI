using BCrypt.Net;

namespace UserAuthAPI.Services
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }

    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
        {
            // Gera um salt e retorna o hash da senha
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            // Verifica se a senha informada corresponde ao hash armazenado
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
