using Microsoft.AspNetCore.Mvc;
using ProjetoEntrevistas.Models;
using ProjetoEntrevistas.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjetoEntrevistas.Controllers;

/// <summary>
/// Controller para gerenciamento de candidatos
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class CandidatosController : ControllerBase
{
    private readonly ICandidatoRepository _repository;

    public CandidatosController(ICandidatoRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Lista todos os candidatos
    /// </summary>
    /// <returns>Lista de candidatos ordenada por nome</returns>
    /// <response code="200">Lista retornada com sucesso</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "📋 Listar todos os candidatos",
        Description = "Retorna lista completa de candidatos ordenada alfabeticamente por nome"
    )]
    [SwaggerResponse(200, "Lista de candidatos", typeof(IEnumerable<Candidato>))]
    public async Task<ActionResult<IEnumerable<Candidato>>> GetAll()
    {
        var candidatos = await _repository.GetAllAsync();
        return Ok(candidatos);
    }

    /// <summary>
    /// Busca um candidato específico por ID
    /// </summary>
    /// <param name="id">ID único do candidato</param>
    /// <returns>Dados do candidato incluindo suas entrevistas</returns>
    /// <response code="200">Candidato encontrado</response>
    /// <response code="404">Candidato não encontrado</response>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "🔍 Buscar candidato por ID",
        Description = "Retorna dados completos do candidato incluindo lista de entrevistas agendadas"
    )]
    [SwaggerResponse(200, "Candidato encontrado", typeof(Candidato))]
    [SwaggerResponse(404, "Candidato não encontrado")]
    public async Task<ActionResult<Candidato>> GetById(Guid id)
    {
        var candidato = await _repository.GetByIdAsync(id);
        
        if (candidato == null)
        {
            return NotFound(new { message = "Candidato não encontrado" });
        }

        return Ok(candidato);
    }

    /// <summary>
    /// Cria um novo candidato
    /// </summary>
    /// <param name="candidato">Dados do candidato (nome obrigatório)</param>
    /// <returns>Candidato criado com ID gerado</returns>
    /// <response code="201">Candidato criado com sucesso</response>
    /// <response code="400">Dados inválidos ou email já cadastrado</response>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/v1/candidatos
    ///     {
    ///       "nome": "Lucas Pereira",
    ///       "email": "lucas@gmail.com",
    ///       "telefone": "(11) 91234-5678"
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
        Summary = "➕ Criar novo candidato",
        Description = "Cadastra um novo candidato no sistema. Email deve ser único se informado."
    )]
    [SwaggerResponse(201, "Candidato criado", typeof(Candidato))]
    [SwaggerResponse(400, "Dados inválidos")]
    public async Task<ActionResult<Candidato>> Create([FromBody] Candidato candidato)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Verificar email duplicado
        if (!string.IsNullOrWhiteSpace(candidato.Email))
        {
            var existente = await _repository.GetByEmailAsync(candidato.Email);
            if (existente != null)
            {
                return BadRequest(new { message = "Email já cadastrado" });
            }
        }

        var criado = await _repository.CreateAsync(candidato);
        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    /// <summary>
    /// Atualiza dados de um candidato existente
    /// </summary>
    /// <param name="id">ID do candidato</param>
    /// <param name="candidato">Novos dados do candidato (sem ID)</param>
    /// <returns>Candidato atualizado</returns>
    /// <response code="200">Candidato atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Candidato não encontrado</response>
    /// <remarks>
    /// **📝 EXEMPLO DE USO:**
    /// 
    ///     PUT /api/v1/candidatos/{id-do-candidato}
    ///     {
    ///       "nome": "Lucas Pereira Silva",
    ///       "email": "lucas.pereira@email.com",
    ///       "telefone": "(11) 91234-5678"
    ///     }
    /// 
    /// **⚠️ IMPORTANTE:**
    /// - **NÃO** envie o campo "id" no JSON (o ID vem da URL)
    /// - Email não pode estar cadastrado para outro candidato
    /// 
    /// **💡 DICA:** Use GET /api/v1/candidatos/{id} para obter os dados atuais antes de atualizar!
    /// </remarks>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "✏️ Atualizar candidato",
        Description = "Atualiza dados de um candidato existente (ID vem da URL, não envie no body)"
    )]
    [SwaggerResponse(200, "Candidato atualizado", typeof(Candidato))]
    [SwaggerResponse(400, "Dados inválidos")]
    [SwaggerResponse(404, "Candidato não encontrado")]
    public async Task<ActionResult<Candidato>> Update(Guid id, [FromBody] Candidato candidato)
    {
        // Define o ID do candidato a partir da URL (não permite alterar)
        candidato.Id = id;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existe = await _repository.ExistsAsync(id);
        if (!existe)
        {
            return NotFound(new { message = "Candidato não encontrado" });
        }

        // Verificar email duplicado (excluindo o próprio candidato)
        if (!string.IsNullOrWhiteSpace(candidato.Email))
        {
            var existente = await _repository.GetByEmailAsync(candidato.Email);
            if (existente != null && existente.Id != id)
            {
                return BadRequest(new { message = "Email já cadastrado para outro candidato" });
            }
        }

        var atualizado = await _repository.UpdateAsync(candidato);
        return Ok(atualizado);
    }

    /// <summary>
    /// Remove um candidato
    /// </summary>
    /// <param name="id">ID do candidato</param>
    /// <returns>Confirmação de remoção</returns>
    /// <response code="204">Candidato removido com sucesso</response>
    /// <response code="400">Candidato possui entrevistas agendadas</response>
    /// <response code="404">Candidato não encontrado</response>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "🗑️ Remover candidato",
        Description = "Remove um candidato. Não é possível remover se houver entrevistas vinculadas."
    )]
    [SwaggerResponse(204, "Candidato removido")]
    [SwaggerResponse(400, "Candidato possui entrevistas")]
    [SwaggerResponse(404, "Candidato não encontrado")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existe = await _repository.ExistsAsync(id);
        if (!existe)
        {
            return NotFound(new { message = "Candidato não encontrado" });
        }

        // Verificar se tem entrevistas
        var hasEntrevistas = await _repository.HasEntrevistasAsync(id);
        if (hasEntrevistas)
        {
            return BadRequest(new { message = "Não é possível remover candidato com entrevistas agendadas" });
        }

        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
