using BoDi;
using Microsoft.Extensions.Options;
using Moq;
using TechTalk.SpecFlow;
using UserAuthAPI.Data;
using UserAuthAPI.Models;
using UserAuthAPI.Repositories;
using UserAuthAPI.Services;

namespace UserAuthAPITest.Hooks
{
    [Binding]
    public class SpecFlowHooks
    {
        private readonly IObjectContainer _objectContainer;

        public SpecFlowHooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario]
        public void RegisterServices()
        {
            // Registrar IUserService no contêiner
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new UserModel { Email = "user@example.com", PasswordHash = "hashedpassword" }); // Mock de retorno

            // Mock para CreateUserAsync
            userServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<UserModel>()))
                .ReturnsAsync((UserModel user) =>
                {
                    // Simulando a criação de um usuário e retornando o usuário com PasswordHash
                    user.PasswordHash = "hashedpassword"; // Supondo que o método HashPassword já foi chamado
                    return user; // Retorna o usuário simulado
                });

            _objectContainer.RegisterInstanceAs(userServiceMock.Object, typeof(IUserService));

            // Registrar IPasswordService no contêiner
            var passwordServiceMock = new Mock<IPasswordService>();
            passwordServiceMock.Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns((string password) => BCrypt.Net.BCrypt.HashPassword(password)); // Simula o hash da senha
            _objectContainer.RegisterInstanceAs(passwordServiceMock.Object, typeof(IPasswordService));

            // Registrar IUserRepository no contêiner
            var userRepositoryMock = new Mock<IUserRepository>();
            _objectContainer.RegisterInstanceAs(userRepositoryMock.Object, typeof(IUserRepository));

            // Registrar DataBaseSettings
            var dbSettings = new DataBaseSettings
            {
                ConnectionURI = "mongodb+srv://wagnersimonetto:1234@cluster0.bb929j4.mongodb.net/user_db?retryWrites=true&w=majority",
                DatabaseName = "user_db",
                UserCollectionName = "users" // Nome da coleção
            };

            // Mock para SendPasswordRecoveryInstructionsAsync
            userServiceMock.Setup(x => x.SendPasswordRecoveryInstructionsAsync(It.IsAny<string>()))
                .ReturnsAsync(true); // Simula que o e-mail foi enviado com sucesso

            // Registrar IOptions<DataBaseSettings>
            _objectContainer.RegisterInstanceAs(Options.Create(dbSettings), typeof(IOptions<DataBaseSettings>));


        }


    }
}
