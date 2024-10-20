using System;
using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using UserAuthAPI.Models;
using UserAuthAPI.Services;

namespace UserAuthAPITest.Steps
{
    [Binding]
    public class UserLogin
    {
        private readonly IUserService _userService;
        private UserModel? _user;
        private Exception? _loginException;

        public UserLogin(IUserService userService)
        {
            _userService = userService;
        }

        [OneTimeSetUp] // Método executado uma vez antes de todos os testes
        public void OneTimeSetup()
        {
            // Inicialização de dependências ou qualquer configuração global necessária
        }

        [SetUp] // Método executado antes de cada teste
        public async Task Setup()
        {
            _user = null;
            _loginException = null;

            // Criar o usuário padrão para os testes
            var email = "user@example.com";
            var password = "senha123"; // Senha sem hash para criar o usuário no mock
            _user = new UserModel { Email = email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(password) };
            await _userService.CreateUserAsync(_user);
        }

        [Given(@"que existe um usuário com email ""(.*)"" e senha ""(.*)""")]
        public async Task GivenQueExisteUmUsuarioComEmailESenha(string email, string password)
        {
            // Use o email fornecido para criar um usuário
            var user = new UserModel
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            await _userService.CreateUserAsync(user);
        }

        [When(@"o usuário tenta fazer login com email ""(.*)"" e senha ""(.*)""")]
        public async Task WhenOUsuarioTentaFazerLoginComEmailESenha(string email, string password)
        {
            try
            {
                _user = await _userService.LoginAsync(email, password);
            }
            catch (Exception ex)
            {
                _loginException = ex;
            }
        }

        [Then(@"o login deve ser bem-sucedido")]
        public void ThenOLoginDeveSerBemSucedido()
        {
            Assert.NotNull(_user, "O usuário não deve ser nulo após o login bem-sucedido.");
            Assert.Null(_loginException, "Não deve ocorrer exceção durante o login.");
        }


    }
}
