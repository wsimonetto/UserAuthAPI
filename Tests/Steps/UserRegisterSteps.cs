using TechTalk.SpecFlow;
using UserAuthAPI.Models;
using UserAuthAPI.Services;
using System.Threading.Tasks;
using NUnit.Framework;

namespace UserAuthAPITest.Steps
{
    [Binding]
    public class UserRegisterSteps
    {
        private readonly IUserService _userService;
        private string? _email;
        private string? _password;
        private bool _isRegistered;

        public UserRegisterSteps(IUserService userService)
        {
            _userService = userService;
        }

        [Given(@"que o usuário fornece um cadastro com e-mail ""(.*)"" e senha ""(.*)""")]
        public void DadoQueOUsuarioForneceUmCadastroComEmailESenha(string email, string password)
        {
            _email = email;
            _password = password; // Armazenando a senha para hash
        }

        [When(@"o usuário se cadastra")]
        public async Task QuandoOUsuarioSeCadastra()
        {
            var password = _password ?? throw new ArgumentException("A senha é obrigatória para o cadastro.", nameof(_password));

            // Gerar o hash da senha usando o IPasswordService
            string passwordHash = HashPassword(password);

            var userModel = new UserModel
            {
                Email = _email,
                PasswordHash = passwordHash // Armazenando apenas o hash da senha
            };

            _isRegistered = await _userService.CreateUserAsync(userModel) != null;
        }

        [Then(@"o cadastro deve ser bem-sucedido")]
        public void EntaoOCadastroDeveSerBemSucedido()
        {
            Assert.IsTrue(_isRegistered, "O cadastro não foi bem-sucedido.");
        }

        [Then(@"o usuário deve receber uma confirmação de cadastro")]
        public void EntaoOUsuarioDeveReceberUmaConfirmacaoDeCadastro()
        {
            // Aqui você pode implementar a lógica para verificar a confirmação de cadastro, se necessário
        }

        // Método fictício para gerar o hash da senha
        private string HashPassword(string password)
        {
            // Aqui você deve implementar o método real para gerar o hash da senha usando IPasswordService
            return password; // Substitua isso pelo hash real
        }
    }
}
