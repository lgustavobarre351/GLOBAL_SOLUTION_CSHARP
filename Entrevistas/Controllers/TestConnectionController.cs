using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoEntrevistas.Data;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjetoEntrevistas.Controllers;

/// <summary>
/// Controller para testar conexão com banco de dados
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class TestConnectionController : ControllerBase
{
    private readonly AppDbContext _context;

    public TestConnectionController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Testa conexão com banco de dados
    /// </summary>
    /// <returns>Status da conexão e estatísticas</returns>
    /// <response code="200">Conexão bem-sucedida</response>
    /// <response code="500">Erro na conexão</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "🔍 Testar conexão com banco",
        Description = "Verifica se a API consegue conectar ao PostgreSQL e retorna estatísticas básicas"
    )]
    [SwaggerResponse(200, "Conexão OK")]
    [SwaggerResponse(500, "Erro na conexão")]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            // Testar conexão
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Não foi possível conectar ao banco de dados"
                });
            }

            // Contar registros
            var empregadores = await _context.Empregadores.CountAsync();
            var candidatos = await _context.Candidatos.CountAsync();
            var entrevistas = await _context.Entrevistas.CountAsync();

            return Ok(new
            {
                success = true,
                message = "Conexão com banco de dados OK!",
                database = new
                {
                    provider = "PostgreSQL",
                    connected = true
                },
                statistics = new
                {
                    empregadores,
                    candidatos,
                    entrevistas
                },
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Erro ao conectar com banco de dados",
                error = ex.Message,
                type = ex.GetType().Name
            });
        }
    }
}