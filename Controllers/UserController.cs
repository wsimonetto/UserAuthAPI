using Microsoft.AspNetCore.Mvc;
using UserAuthAPI.Models;
using UserAuthAPI.Services;
using UserAuthAPI.Requests; // Importar o namespace para LoginRequest

namespace UserAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        [Route("cadastrar")]
        public async Task<ActionResult<UserModel>> Cadastrar([FromBody] UserModel user)
        {
            // Validação dos dados
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
            {
                _logger.LogWarning("Tentativa de cadastro com email ou senha ausente.");
                return BadRequest("Email e senha são obrigatórios.");
            }

            var existingUser = await _userService.GetUserByEmailAsync(user.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Tentativa de cadastro com email já existente: {Email}", user.Email);
                return BadRequest("E-mail já cadastrado.");
            }

            try
            {
                await _userService.CreateUserAsync(user);
                _logger.LogInformation("Usuário cadastrado com sucesso: {Email}", user.Email);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar usuário: {Email}", user.Email);
                return StatusCode(500, $"Erro ao cadastrar usuário: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado: {Id}", id);
                return NotFound();
            }
            _logger.LogInformation("Usuário encontrado: {Id}", id);
            return Ok(user);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserModel>> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado com o email: {Email}", email);
                return NotFound();
            }
            _logger.LogInformation("Usuário encontrado: {Email}", email);
            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel user)
        {
            var updated = await _userService.UpdateUserAsync(user);
            if (!updated)
            {
                _logger.LogWarning("Falha ao atualizar o usuário: {Id}", user.Id);
                return NotFound();
            }
            _logger.LogInformation("Usuário atualizado com sucesso: {Id}", user.Id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Falha ao deletar o usuário: {Id}", id);
                return NotFound();
            }
            _logger.LogInformation("Usuário deletado com sucesso: {Id}", id);
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning("Tentativa de login com email ou senha ausente.");
                return BadRequest("Email e senha são obrigatórios.");
            }

            try
            {
                var user = await _userService.LoginAsync(request.Email, request.Password);
                if (user == null)
                {
                    _logger.LogWarning("Tentativa de login falhou para o email: {Email}", request.Email);
                    return Unauthorized("Email ou senha incorretos.");
                }

                _logger.LogInformation("Login bem-sucedido para o usuário: {Email}", request.Email);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar login.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao processar a solicitação.");
            }
        }

        [HttpPost("recover-password")]
        public async Task<IActionResult> RecoverPassword([FromBody] EmailRequest emailRequest)
        {
            if (emailRequest == null || string.IsNullOrEmpty(emailRequest.Email))
            {
                _logger.LogWarning("Tentativa de recuperação de senha com email ausente.");
                return BadRequest("Email é obrigatório.");
            }

            var success = await _userService.SendPasswordRecoveryInstructionsAsync(emailRequest.Email);
            if (!success)
            {
                _logger.LogWarning("Tentativa de recuperação de senha falhou para o email: {Email}", emailRequest.Email);
                return NotFound("Usuário não encontrado.");
            }

            _logger.LogInformation("Instruções de recuperação de senha enviadas para o email: {Email}", emailRequest.Email);
            return Ok("E-mail de recuperação enviado com sucesso.");
        }


    }
}
