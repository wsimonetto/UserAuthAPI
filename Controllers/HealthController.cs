using Microsoft.AspNetCore.Mvc;
using UserAuthAPI.Data;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly MongoDBContext _mongoDBContext;

    public HealthController(MongoDBContext mongoDBContext)
    {
        _mongoDBContext = mongoDBContext;
    }

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            var collections = _mongoDBContext.Users; // Testa o acesso à coleção
            return Ok("Conexão com o MongoDB bem-sucedida.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao conectar ao MongoDB: {ex.Message}");
        }
    }
}
