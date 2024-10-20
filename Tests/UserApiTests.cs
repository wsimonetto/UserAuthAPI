using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UserAuthAPI.Models;

namespace UserAuthAPITest
{
    public class UserApiTests
    {
        private HttpClient _client = new HttpClient { BaseAddress = new Uri("https://atividadebdd.azurewebsites.net/") };


        [SetUp]
        public void Setup()
        {
            _client = new HttpClient { BaseAddress = new Uri("https://atividadebdd.azurewebsites.net/") };
        }

        [TearDown]
        public void TearDown()
        {
            // Liberar recursos do HttpClient após os testes
            _client.Dispose();
        }

        [Test]
        public async Task Test_CadastroUsuario_EmailJaCadastrado()
        {
            // Gera um e-mail unico através do horario
            var uniqueEmail = $"user_{DateTime.UtcNow.Ticks}@example.com";

            var user = new UserModel
            {
                Email = uniqueEmail,
                PasswordHash = "senhaSegura123",
                RecoveryEmail = "recovery@example.com"
            };

            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            // Cadastra o usuário pela primeira vez
            var response = await _client.PostAsync("api/user/cadastrar", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            TestContext.WriteLine($"Resposta do cadastro inicial: {responseBody}");

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, $"Esperado: Created, Mas foi: {response.StatusCode}");

            // Tenta cadastrar novamente o mesmo e-mail
            var responseDuplicate = await _client.PostAsync("api/user/cadastrar", content);
            var responseDuplicateBody = await responseDuplicate.Content.ReadAsStringAsync();
            TestContext.WriteLine($"Resposta do cadastro duplicado: {responseDuplicateBody}");

            // Validação do status code para e-mail já cadastrado
            Assert.AreEqual(HttpStatusCode.BadRequest, responseDuplicate.StatusCode, $"Esperado: BadRequest, Mas foi: {responseDuplicate.StatusCode}");

            // Validação do corpo da resposta
            Assert.IsTrue(responseDuplicateBody.Contains("E-mail já cadastrado"), "A resposta não contém a mensagem esperada para e-mail duplicado.");
        }

        [Test]
        public async Task Test_RecuperacaoSenha_EmailInvalido()
        {
            var email = "invalido@example.com"; // Enviar uma string simples
            var content = new StringContent($"\"{email}\"", Encoding.UTF8, "application/json"); // Envie uma string JSON

            var response = await _client.PostAsync("api/user/recover-password", content);

            // Log para verificar a resposta ao tentar recuperar senha com e-mail inválido
            var responseBody = await response.Content.ReadAsStringAsync();
            TestContext.WriteLine("Resposta de recuperação de senha com e-mail inválido: " + responseBody);

            // Verifica se o status code retornado é BadRequest
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, $"Esperado: BadRequest, Mas foi: {response.StatusCode}");
            Assert.IsTrue(responseBody.Contains("Usuário não encontrado.") || responseBody.Contains("The emailRequest field is required."),
                "A resposta não contém a mensagem esperada para e-mail inválido.");
        }

        [Test]
        public async Task Test_CadastroUsuario_EstruturaCorreta()
        {
            // Gera um e-mail unico através do horario
            var uniqueEmail = $"user_{DateTime.UtcNow.Ticks}@example.com";

            var user = new
            {
                email = uniqueEmail,
                passwordHash = "senhaSegura123"   // Altere para 'passwordHash'
                                                  // Adicione outros campos se necessário, como 'recoveryEmail'
            };

            // Serializa o objeto em JSON
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            // Envia a solicitação POST para a API
            var response = await _client.PostAsync("api/user/cadastrar", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Log para verificar a resposta do cadastro
            TestContext.WriteLine("Resposta do cadastro para estrutura correta: " + responseBody);

            // Verifica se o status code retornado é Created
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, $"Esperado: Created, Mas foi: {response.StatusCode}");

            // Defina seu esquema JSON, refletindo a estrutura correta da resposta
            var schema = JSchema.Parse(@"{
            'type': 'object',
            'properties': {
                'email': { 'type': 'string' },
                'id': { 'type': 'string' }
            },
            'required': ['email', 'id']
            }");

            // Verifica se a resposta está em formato JSON
            var responseJson = JToken.Parse(responseBody);

            // Valida a estrutura do JSON
            var validationResult = responseJson.IsValid(schema, out IList<string> errorMessages);
            Assert.IsTrue(validationResult, "Erros na validação do esquema JSON: " + string.Join(", ", errorMessages));
            //
        }


        [Test]
        public async Task Test_Login_Success()
        {
            // Dados do usuário de teste com credenciais válidas
            var loginRequest = new
            {
                email = "user_638649430976376221@example.com",
                password = "senhaSegura123"
            };

            // Serializa o objeto em JSON
            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            // Envia a solicitação POST para a API de login
            var response = await _client.PostAsync("api/user/login", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Log para verificar a resposta do login
            TestContext.WriteLine("Resposta do login com credenciais válidas: " + responseBody);

            // Verifica se o status code retornado é OK
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Esperado: OK, Mas foi: {response.StatusCode}");
        }

        [Test]
        public async Task Test_Login_InvalidCredentials()
        {
            // Dados do usuário de teste com credenciais inválidas
            var loginRequest = new
            {
                email = "user_invalid@example.com",
                password = "senhaErrada"
            };

            // Serializa o objeto em JSON
            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            // Envia a solicitação POST para a API de login
            var response = await _client.PostAsync("api/user/login", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Log para verificar a resposta do login com credenciais inválidas
            TestContext.WriteLine("Resposta do login com credenciais inválidas: " + responseBody);

            // Verifica se o status code retornado é Unauthorized
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, $"Esperado: Unauthorized, Mas foi: {response.StatusCode}");

            // Validação opcional para mensagem de erro no corpo da resposta
            Assert.IsTrue(responseBody.Contains("Email ou senha incorretos"), "A resposta não contém a mensagem esperada para credenciais inválidas.");
        }


    }
}
