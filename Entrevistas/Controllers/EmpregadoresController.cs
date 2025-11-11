using Microsoft.AspNetCore.Mvc;
using ProjetoEntrevistas.Models;
using ProjetoEntrevistas.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjetoEntrevistas.Controllers;

/// <summary>
/// Controller para gerenciamento de empregadores/recrutadores
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class EmpregadoresController : ControllerBase
{
    private readonly IEmpregadorRepository _repository;

    public EmpregadoresController(IEmpregadorRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Lista todos os empregadores
    /// </summary>
    /// <returns>Lista de empregadores ordenada por nome</returns>
    /// <response code="200">Lista retornada com sucesso</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "📋 Listar todos os empregadores",
        Description = "Retorna lista completa de empregadores ordenada alfabeticamente por nome"
    )]
    [SwaggerResponse(200, "Lista de empregadores", typeof(IEnumerable<Empregador>))]
    public async Task<ActionResult<IEnumerable<Empregador>>> GetAll()
    {
        var empregadores = await _repository.GetAllAsync();
        return Ok(empregadores);
    }

    /// <summary>
    /// Busca um empregador específico por ID
    /// </summary>
    /// <param name="id">ID único do empregador</param>
    /// <returns>Dados do empregador incluindo suas entrevistas</returns>
    /// <response code="200">Empregador encontrado</response>
    /// <response code="404">Empregador não encontrado</response>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "🔍 Buscar empregador por ID",
        Description = "Retorna dados completos do empregador incluindo lista de entrevistas agendadas"
    )]
    [SwaggerResponse(200, "Empregador encontrado", typeof(Empregador))]
    [SwaggerResponse(404, "Empregador não encontrado")]
    public async Task<ActionResult<Empregador>> GetById(Guid id)
    {
        var empregador = await _repository.GetByIdAsync(id);
        
        if (empregador == null)
        {
            return NotFound(new { message = "Empregador não encontrado" });
        }

        return Ok(empregador);
    }

    /// <summary>
    /// Cria um novo empregador
    /// </summary>
    /// <param name="empregador">Dados do empregador (nome obrigatório)</param>
    /// <returns>Empregador criado com ID gerado</returns>
    /// <response code="201">Empregador criado com sucesso</response>
    /// <response code="400">Dados inválidos ou email já cadastrado</response>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/v1/empregadores
    ///     {
    ///       "nome": "Ana Souza",
    ///       "email": "ana@empresa.com",
    ///       "telefone": "(11) 99999-1111"
    ///     }
    /// 
    /// **Campos obrigatórios:** `nome`
    /// 
    /// **Regras:**
    /// - Email deve ser único (se informado)
    /// - Nome pode ter até 200 caracteres
    /// - ID será gerado automaticamente
    /// </remarks>
    [HttpPost]
    [SwaggerOperation(
        Summary = "➕ Criar novo empregador",
        Description = "Cadastra um novo empregador no sistema. Email deve ser único se informado."
    )]
    [SwaggerResponse(201, "Empregador criado", typeof(Empregador))]
    [SwaggerResponse(400, "Dados inválidos")]
    public async Task<ActionResult<Empregador>> Create([FromBody] Empregador empregador)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Verificar email duplicado
        if (!string.IsNullOrWhiteSpace(empregador.Email))
        {
            var existente = await _repository.GetByEmailAsync(empregador.Email);
            if (existente != null)
            {
                return BadRequest(new { message = "Email já cadastrado" });
            }
        }

        var criado = await _repository.CreateAsync(empregador);
        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    /// <summary>
    /// Atualiza dados de um empregador existente
    /// </summary>
    /// <param name="id">ID do empregador</param>
    /// <param name="empregador">Novos dados do empregador (sem ID)</param>
    /// <returns>Empregador atualizado</returns>
    /// <response code="200">Empregador atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Empregador não encontrado</response>
    /// <remarks>
    /// **📝 EXEMPLO DE USO:**
    /// 
    ///     PUT /api/v1/empregadores/{id-do-empregador}
    ///     {
    ///       "nome": "Ana Souza Silva",
    ///       "email": "ana.souza@empresa.com.br",
    ///       "telefone": "(11) 99999-1111"
    ///     }
    /// 
    /// **⚠️ IMPORTANTE:**
    /// - **NÃO** envie o campo "id" no JSON (o ID vem da URL)
    /// - Email não pode estar cadastrado para outro empregador
    /// 
    /// **💡 DICA:** Use GET /api/v1/empregadores/{id} para obter os dados atuais antes de atualizar!
    /// </remarks>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "✏️ Atualizar empregador",
        Description = "Atualiza dados de um empregador existente (ID vem da URL, não envie no body)"
    )]
    [SwaggerResponse(200, "Empregador atualizado", typeof(Empregador))]
    [SwaggerResponse(400, "Dados inválidos")]
    [SwaggerResponse(404, "Empregador não encontrado")]
    public async Task<ActionResult<Empregador>> Update(Guid id, [FromBody] Empregador empregador)
    {
        // Define o ID do empregador a partir da URL (não permite alterar)
        empregador.Id = id;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existe = await _repository.ExistsAsync(id);
        if (!existe)
        {
            return NotFound(new { message = "Empregador não encontrado" });
        }

        // Verificar email duplicado (excluindo o próprio empregador)
        if (!string.IsNullOrWhiteSpace(empregador.Email))
        {
            var existente = await _repository.GetByEmailAsync(empregador.Email);
            if (existente != null && existente.Id != id)
            {
                return BadRequest(new { message = "Email já cadastrado para outro empregador" });
            }
        }

        var atualizado = await _repository.UpdateAsync(empregador);
        return Ok(atualizado);
    }

    /// <summary>
    /// Remove um empregador
    /// </summary>
    /// <param name="id">ID do empregador</param>
    /// <returns>Confirmação de remoção</returns>
    /// <response code="204">Empregador removido com sucesso</response>
    /// <response code="400">Empregador possui entrevistas agendadas</response>
    /// <response code="404">Empregador não encontrado</response>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "🗑️ Remover empregador",
        Description = "Remove um empregador. Não é possível remover se houver entrevistas vinculadas."
    )]
    [SwaggerResponse(204, "Empregador removido")]
    [SwaggerResponse(400, "Empregador possui entrevistas")]
    [SwaggerResponse(404, "Empregador não encontrado")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existe = await _repository.ExistsAsync(id);
        if (!existe)
        {
            return NotFound(new { message = "Empregador não encontrado" });
        }

        // Verificar se tem entrevistas
        var hasEntrevistas = await _repository.HasEntrevistasAsync(id);
        if (hasEntrevistas)
        {
            return BadRequest(new { message = "Não é possível remover empregador com entrevistas agendadas" });
        }

        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
