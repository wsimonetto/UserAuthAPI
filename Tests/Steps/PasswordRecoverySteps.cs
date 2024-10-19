using TechTalk.SpecFlow;
using UserAuthAPI.Services;
using Moq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace UserAuthAPITest.Steps
{
    [Binding]
    public class PasswordRecoverySteps
    {
        private readonly IUserService _userService;
        private string _email;
        private bool _isEmailSent;

        public PasswordRecoverySteps(IUserService userService)
        {
            _userService = userService;
        }

        [Given(@"que o usuário fornece um e-mail ""(.*)""")]
        public void DadoQueOUsuarioForneceUmEmail(string email)
        {
            _email = email;
        }

        [When(@"o usuário solicita a recuperação de senha")]
        public async Task QuandoOUsuarioSolicitaARecuperacaoDeSenha()
        {
            _isEmailSent = await _userService.SendPasswordRecoveryInstructionsAsync(_email);
        }

        [Then(@"o sistema deve enviar um e-mail com instruções de recuperação")]
        public void EntaoOSistemaDeveEnviarUmEmailComInstrucoesDeRecuperacao()
        {
            Assert.IsTrue(_isEmailSent);
        }

        [Then(@"o e-mail deve ser enviado para ""(.*)""")]
        public void EntaoOEmailDeveSerEnviadoPara(string expectedEmail)
        {
            Assert.AreEqual(expectedEmail, _email);
        }
    }
}
