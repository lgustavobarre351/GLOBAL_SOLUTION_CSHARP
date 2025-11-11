using Microsoft.AspNetCore.Mvc;
using ProjetoEntrevistas.Models;
using ProjetoEntrevistas.Models.Enums;
using ProjetoEntrevistas.Repositories;
using ProjetoEntrevistas.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjetoEntrevistas.Controllers;

/// <summary>
/// Controller para gerenciamento de entrevistas
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class EntrevistasController : ControllerBase
{
    private readonly IEntrevistaRepository _repository;
    private readonly IEntrevistaValidationService _validationService;

    public EntrevistasController(
        IEntrevistaRepository repository,
        IEntrevistaValidationService validationService)
    {
        _repository = repository;
        _validationService = validationService;
    }

    /// <summary>
    /// Lista todas as entrevistas
    /// </summary>
    /// <returns>Lista de entrevistas com dados de empregador e candidato</returns>
    /// <response code="200">Lista retornada com sucesso</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "📋 Listar todas as entrevistas [LINQ]",
        Description = "🔍 CONSULTA LINQ: OrderByDescending() + Include() - Retorna todas as entrevistas ordenadas por data (mais recentes primeiro)"
    )]
    [SwaggerResponse(200, "Lista de entrevistas", typeof(IEnumerable<Entrevista>))]
    public async Task<ActionResult<IEnumerable<Entrevista>>> GetAll()
    {
        var entrevistas = await _repository.GetAllAsync();
        return Ok(entrevistas);
    }

    /// <summary>
    /// Busca uma entrevista por ID
    /// </summary>
    /// <param name="id">ID único da entrevista</param>
    /// <returns>Dados completos da entrevista</returns>
    /// <response code="200">Entrevista encontrada</response>
    /// <response code="404">Entrevista não encontrada</response>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "🔍 Buscar entrevista por ID",
        Description = "Retorna dados completos da entrevista incluindo empregador e candidato"
    )]
    [SwaggerResponse(200, "Entrevista encontrada", typeof(Entrevista))]
    [SwaggerResponse(404, "Entrevista não encontrada")]
    public async Task<ActionResult<Entrevista>> GetById(Guid id)
    {
        var entrevista = await _repository.GetByIdAsync(id);
        
        if (entrevista == null)
        {
            return NotFound(new { message = "Entrevista não encontrada" });
        }

        return Ok(entrevista);
    }

    /// <summary>
    /// Lista entrevistas de um empregador específico
    /// </summary>
    /// <param name="id">ID do empregador</param>
    /// <returns>Lista de entrevistas agendadas pelo empregador</returns>
    /// <response code="200">Lista retornada</response>
    [HttpGet("empregador/{id:guid}")]
    [SwaggerOperation(
        Summary = "👔 Entrevistas por empregador [LINQ]",
        Description = "🔍 CONSULTA LINQ: Where() + OrderByDescending() - Filtra entrevistas de um empregador específico"
    )]
    [SwaggerResponse(200, "Lista de entrevistas do empregador", typeof(IEnumerable<Entrevista>))]
    public async Task<ActionResult<IEnumerable<Entrevista>>> GetByEmpregador(Guid id)
    {
        var entrevistas = await _repository.GetByEmpregadorAsync(id);
        return Ok(entrevistas);
    }

    /// <summary>
    /// Lista entrevistas de um candidato específico
    /// </summary>
    /// <param name="id">ID do candidato</param>
    /// <returns>Lista de entrevistas do candidato</returns>
    /// <response code="200">Lista retornada</response>
    [HttpGet("candidato/{id:guid}")]
    [SwaggerOperation(
        Summary = "👤 Entrevistas por candidato [LINQ]",
        Description = "🔍 CONSULTA LINQ: Where() + OrderByDescending() - Filtra entrevistas de um candidato específico"
    )]
    [SwaggerResponse(200, "Lista de entrevistas do candidato", typeof(IEnumerable<Entrevista>))]
    public async Task<ActionResult<IEnumerable<Entrevista>>> GetByCandidato(Guid id)
    {
        var entrevistas = await _repository.GetByCandidatoAsync(id);
        return Ok(entrevistas);
    }

    /// <summary>
    /// Filtra entrevistas por status
    /// </summary>
    /// <param name="status">Status: Agendada, Cancelada ou Concluida</param>
    /// <returns>Lista de entrevistas com o status especificado</returns>
    /// <response code="200">Lista retornada</response>
    /// <remarks>
    /// **📊 EXEMPLOS DE USO:**
    /// 
    ///     GET /api/v1/entrevistas/status/Agendada
    ///     GET /api/v1/entrevistas/status/Concluida
    ///     GET /api/v1/entrevistas/status/Cancelada
    /// 
    /// **📋 VALORES VÁLIDOS:**
    /// - `Agendada` - Entrevistas futuras confirmadas
    /// - `Concluida` - Entrevistas já realizadas
    /// - `Cancelada` - Entrevistas canceladas
    /// 
    /// **💡 USO PRÁTICO:**
    /// - Ver apenas entrevistas ativas: status=Agendada
    /// - Histórico de entrevistas: status=Concluida
    /// - Análise de cancelamentos: status=Cancelada
    /// </remarks>
    [HttpGet("status/{status}")]
    [SwaggerOperation(
        Summary = "📊 Filtrar por status [LINQ]",
        Description = "🔍 CONSULTA LINQ: Where() com enum - Filtra entrevistas por status (Agendada/Cancelada/Concluida)"
    )]
    [SwaggerResponse(200, "Lista filtrada", typeof(IEnumerable<Entrevista>))]
    public async Task<ActionResult<IEnumerable<Entrevista>>> GetByStatus(StatusEntrevista status)
    {
        var entrevistas = await _repository.GetByStatusAsync(status);
        return Ok(entrevistas);
    }

    /// <summary>
    /// Filtra entrevistas por tipo
    /// </summary>
    /// <param name="tipo">Tipo: Online, Presencial ou Telefone</param>
    /// <returns>Lista de entrevistas do tipo especificado</returns>
    /// <response code="200">Lista retornada</response>
    /// <remarks>
    /// **🎯 EXEMPLOS DE USO:**
    /// 
    ///     GET /api/v1/entrevistas/tipo/Online
    ///     GET /api/v1/entrevistas/tipo/Presencial
    ///     GET /api/v1/entrevistas/tipo/Telefone
    /// 
    /// **📋 VALORES VÁLIDOS:**
    /// - `Online` - Entrevistas via videoconferência (Teams, Meet, Zoom)
    /// - `Presencial` - Entrevistas no escritório/local físico
    /// - `Telefone` - Entrevistas por chamada telefônica
    /// 
    /// **💡 USO PRÁTICO:**
    /// - Filtrar entrevistas online para enviar lembretes com links
    /// - Ver quantas entrevistas presenciais para reservar salas
    /// - Separar entrevistas telefônicas para ligar no horário
    /// </remarks>
    [HttpGet("tipo/{tipo}")]
    [SwaggerOperation(
        Summary = "🎯 Filtrar por tipo [LINQ]",
        Description = "🔍 CONSULTA LINQ: Where() com enum - Filtra entrevistas por tipo (Online/Presencial/Telefone)"
    )]
    [SwaggerResponse(200, "Lista filtrada", typeof(IEnumerable<Entrevista>))]
    public async Task<ActionResult<IEnumerable<Entrevista>>> GetByTipo(TipoEntrevista tipo)
    {
        var entrevistas = await _repository.GetByTipoAsync(tipo);
        return Ok(entrevistas);
    }

    /// <summary>
    /// Agenda do dia - entrevistas em uma data específica
    /// </summary>
    /// <param name="data">Data no formato YYYY-MM-DD</param>
    /// <returns>Lista de entrevistas do dia ordenada por horário</returns>
    /// <response code="200">Agenda do dia</response>
    /// <remarks>
    /// **📅 EXEMPLOS DE USO:**
    /// 
    ///     GET /api/v1/entrevistas/agenda/2025-11-15
    ///     GET /api/v1/entrevistas/agenda/2025-12-01
    /// 
    /// **📋 RETORNA:**
    /// - Todas as entrevistas do dia especificado
    /// - Ordenadas por horário (mais cedo primeiro)
    /// - Inclui todos os detalhes (empregador, candidato, tipo, status, etc)
    /// 
    /// **💡 USO PRÁTICO:**
    /// - Ver agenda diária do RH
    /// - Planejar logística de entrevistas
    /// - Verificar disponibilidade de salas/horários
    /// </remarks>
    [HttpGet("agenda/{data:datetime}")]
    [SwaggerOperation(
        Summary = "📅 Agenda do dia [LINQ]",
        Description = "🔍 CONSULTA LINQ: Where() com range de datas + OrderBy() - Retorna todas as entrevistas de um dia específico"
    )]
    [SwaggerResponse(200, "Entrevistas do dia", typeof(IEnumerable<Entrevista>))]
    public async Task<ActionResult<IEnumerable<Entrevista>>> GetByData(DateTime data)
    {
        var entrevistas = await _repository.GetByDataAsync(data);
        return Ok(entrevistas);
    }

    /// <summary>
    /// Dashboard estatístico de entrevistas
    /// </summary>
    /// <returns>Estatísticas agregadas (total, por status, por tipo, próximas)</returns>
    /// <response code="200">Dashboard com estatísticas</response>
    /// <remarks>
    /// **📊 EXEMPLO DE RETORNO:**
    /// 
    ///     {
    ///       "totalEntrevistas": 15,
    ///       "porStatus": {
    ///         "agendadas": 8,
    ///         "concluidas": 5,
    ///         "canceladas": 2
    ///       },
    ///       "porTipo": {
    ///         "online": 10,
    ///         "presencial": 3,
    ///         "telefone": 2
    ///       },
    ///       "proximasSemana": 5,
    ///       "duracaoMedia": 52.5
    ///     }
    /// 
    /// **📋 O QUE RETORNA:**
    /// - **totalEntrevistas**: Total geral de entrevistas
    /// - **porStatus**: Contagem por cada status (agendadas, concluídas, canceladas)
    /// - **porTipo**: Contagem por tipo (online, presencial, telefone)
    /// - **proximasSemana**: Entrevistas agendadas nos próximos 7 dias
    /// - **duracaoMedia**: Duração média em minutos de todas as entrevistas
    /// 
    /// **💡 USO PRÁTICO:**
    /// - Visão geral do sistema de entrevistas
    /// - KPIs para gestão de RH
    /// - Relatório executivo rápido
    /// - Monitoramento de volume de entrevistas
    /// </remarks>
    [HttpGet("dashboard")]
    [SwaggerOperation(
        Summary = "📊 Dashboard estatístico [LINQ]",
        Description = "🔍 CONSULTAS LINQ: GroupBy() + Count() + Average() + Sum() - Retorna estatísticas agregadas das entrevistas"
    )]
    [SwaggerResponse(200, "Dashboard com estatísticas")]
    public async Task<ActionResult<Dictionary<string, object>>> GetDashboard()
    {
        var dashboard = await _repository.GetDashboardAsync();
        return Ok(dashboard);
    }

    /// <summary>
    /// Agenda uma nova entrevista
    /// </summary>
    /// <param name="entrevista">Dados da entrevista</param>
    /// <returns>Entrevista criada</returns>
    /// <response code="201">Entrevista agendada com sucesso</response>
    /// <response code="400">Dados inválidos ou conflito de horário</response>
    /// <remarks>
    /// **📝 EXEMPLOS DE USO POR TIPO:**
    /// 
    /// ---
    /// 
    /// **1️⃣ ENTREVISTA ONLINE** (requer `linkReuniao`):
    /// 
    ///     POST /api/v1/entrevistas
    ///     {
    ///       "idEmpregador": "uuid-do-empregador",
    ///       "idCandidato": "uuid-do-candidato",
    ///       "dataHora": "2025-11-15T10:00:00Z",
    ///       "duracaoMinutos": 60,
    ///       "tipo": 0,
    ///       "status": 0,
    ///       "linkReuniao": "https://meet.google.com/abc-defg-hij",
    ///       "observacoes": "Entrevista técnica inicial"
    ///     }
    /// 
    /// ---
    /// 
    /// **2️⃣ ENTREVISTA PRESENCIAL** (requer `local`):
    /// 
    ///     POST /api/v1/entrevistas
    ///     {
    ///       "idEmpregador": "uuid-do-empregador",
    ///       "idCandidato": "uuid-do-candidato",
    ///       "dataHora": "2025-11-16T14:30:00Z",
    ///       "duracaoMinutos": 45,
    ///       "tipo": 1,
    ///       "status": 0,
    ///       "local": "Escritório - Rua das Flores, 100",
    ///       "observacoes": "Trazer documentos"
    ///     }
    /// 
    /// ---
    /// 
    /// **3️⃣ ENTREVISTA POR TELEFONE** (apenas campos básicos):
    /// 
    ///     POST /api/v1/entrevistas
    ///     {
    ///       "idEmpregador": "uuid-do-empregador",
    ///       "idCandidato": "uuid-do-candidato",
    ///       "dataHora": "2025-11-17T09:00:00Z",
    ///       "duracaoMinutos": 30,
    ///       "tipo": 2,
    ///       "status": 0,
    ///       "observacoes": "Ligar no número cadastrado"
    ///     }
    /// 
    /// ---
    /// 
    /// **⚠️ VALIDAÇÕES AUTOMÁTICAS:**
    /// - ✅ Empregador e candidato devem existir
    /// - ✅ Data/hora não pode ser no passado
    /// - ✅ Duração entre 15 e 480 minutos
    /// - ✅ `linkReuniao` obrigatório se tipo = 0 (Online)
    /// - ✅ `local` obrigatório se tipo = 1 (Presencial)
    /// - ✅ Verifica conflitos de horário do candidato
    /// - ✅ Horário comercial (07:00 - 22:00)
    /// 
    /// ---
    /// 
    /// **📋 ATENÇÃO: COMO PREENCHER OS CAMPOS TIPO E STATUS**
    /// 
    /// **Campo "tipo" - Digite apenas o NÚMERO:**
    /// - Digite **0** = online (videochamada - OBRIGATÓRIO campo linkReuniao)
    /// - Digite **1** = presencial (escritório - OBRIGATÓRIO campo local)
    /// - Digite **2** = telefone (ligação telefônica - apenas campos básicos)
    /// 
    /// **Campo "status" - Digite apenas o NÚMERO:**
    /// - Digite **0** = scheduled/agendada (padrão para novas entrevistas)
    /// - Digite **1** = canceled/cancelada
    /// - Digite **2** = completed/concluída
    /// 
    /// **IMPORTANTE:** Use apenas os números (0, 1 ou 2) sem aspas ou texto!
    /// </remarks>
    [HttpPost]
    [SwaggerOperation(
        Summary = "➕ Agendar nova entrevista",
        Description = "Cria uma nova entrevista com validações: empregador/candidato existem, sem conflitos de horário, campos obrigatórios por tipo"
    )]
    [SwaggerResponse(201, "Entrevista criada", typeof(Entrevista))]
    [SwaggerResponse(400, "Dados inválidos ou validação falhou")]
    public async Task<ActionResult<Entrevista>> Create([FromBody] Entrevista entrevista)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validar regras de negócio
        var (isValid, errorMessage) = await _validationService.ValidateEntrevistaAsync(entrevista);
        if (!isValid)
        {
            return BadRequest(new { message = errorMessage });
        }

        var criada = await _repository.CreateAsync(entrevista);
        return CreatedAtAction(nameof(GetById), new { id = criada.Id }, criada);
    }

    /// <summary>
    /// Obtém dados da entrevista formatados para edição
    /// </summary>
    /// <param name="id">ID da entrevista</param>
    /// <returns>Dados atuais da entrevista em formato editável</returns>
    /// <response code="200">Dados para edição</response>
    /// <response code="404">Entrevista não encontrada</response>
    /// <remarks>
    /// **📋 COMO USAR:**
    /// 
    /// 1️⃣ Chame este endpoint para ver os dados atuais
    /// 2️⃣ Copie o JSON retornado
    /// 3️⃣ Cole no PUT /api/v1/entrevistas/{id}
    /// 4️⃣ Edite apenas os campos que deseja alterar
    /// 5️⃣ Execute o PUT
    /// 
    /// **💡 VANTAGEM:** Você vê exatamente como estão os dados antes de modificar!
    /// </remarks>
    [HttpGet("{id:guid}/editar")]
    [SwaggerOperation(
        Summary = "📝 Ver dados para editar",
        Description = "Retorna os dados atuais da entrevista formatados para você copiar e editar no PUT"
    )]
    [SwaggerResponse(200, "Dados prontos para edição")]
    [SwaggerResponse(404, "Entrevista não encontrada")]
    public async Task<ActionResult<object>> GetParaEditar(Guid id)
    {
        var entrevista = await _repository.GetByIdAsync(id);
        if (entrevista == null)
        {
            return NotFound(new { message = "Entrevista não encontrada" });
        }

        // Retornar formato simplificado sem navegação circular
        var dadosParaEditar = new
        {
            idEmpregador = entrevista.IdEmpregador,
            idCandidato = entrevista.IdCandidato,
            dataHora = entrevista.DataHora,
            duracaoMinutos = entrevista.DuracaoMinutos,
            tipo = (int)entrevista.Tipo,
            status = (int)entrevista.Status,
            linkReuniao = entrevista.LinkReuniao,
            local = entrevista.Local,
            observacoes = entrevista.Observacoes,
            _INSTRUCAO = "👆 Copie este JSON, cole no PUT /api/v1/entrevistas/" + id + " e edite o que quiser!"
        };

        return Ok(dadosParaEditar);
    }

    /// <summary>
    /// Atualiza uma entrevista existente
    /// </summary>
    /// <param name="id">ID da entrevista</param>
    /// <param name="entrevista">Novos dados da entrevista (sem ID)</param>
    /// <returns>Entrevista atualizada</returns>
    /// <response code="200">Entrevista atualizada</response>
    /// <response code="400">Dados inválidos ou conflito</response>
    /// <response code="404">Entrevista não encontrada</response>
    /// <remarks>
    /// **📝 EXEMPLO - Remarcar entrevista online:**
    /// 
    ///     PUT /api/v1/entrevistas/{id-da-entrevista}
    ///     {
    ///       "idEmpregador": "id-do-empregador",
    ///       "idCandidato": "id-do-candidato",
    ///       "dataHora": "2025-11-20T15:00:00Z",
    ///       "duracaoMinutos": 90,
    ///       "tipo": "Online",
    ///       "status": "Agendada",
    ///       "linkReuniao": "https://zoom.us/j/123456789",
    ///       "observacoes": "Reagendado - nova data combinada"
    ///     }
    /// 
    /// **📝 EXEMPLO - Alterar status para concluída:**
    /// 
    ///     PUT /api/v1/entrevistas/{id-da-entrevista}
    ///     {
    ///       "idEmpregador": "id-do-empregador",
    ///       "idCandidato": "id-do-candidato",
    ///       "dataHora": "2025-11-15T10:00:00Z",
    ///       "duracaoMinutos": 60,
    ///       "tipo": "Online",
    ///       "status": "Concluida",
    ///       "linkReuniao": "https://meet.google.com/abc-defg-hij",
    ///       "observacoes": "Candidato aprovado para próxima fase"
    ///     }
    /// 
    /// **⚠️ IMPORTANTE:**
    /// - **NÃO** envie o campo "id" no JSON (o ID vem da URL)
    /// - Todas as mesmas validações do POST são aplicadas
    /// - Não cria conflito com a própria entrevista ao verificar horários
    /// 
    /// **💡 DICA:** Use GET /api/v1/entrevistas/{id} para obter os dados atuais antes de atualizar!
    /// </remarks>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "✏️ Atualizar entrevista",
        Description = "⚠️ ANTES DE ATUALIZAR: Use GET /api/v1/entrevistas/{id}/editar para ver os dados atuais e copiar para cá!"
    )]
    [SwaggerResponse(200, "Entrevista atualizada", typeof(Entrevista))]
    [SwaggerResponse(400, "Dados inválidos")]
    [SwaggerResponse(404, "Entrevista não encontrada")]
    public async Task<ActionResult<Entrevista>> Update(Guid id, [FromBody] Entrevista entrevista)
    {
        // Define o ID da entrevista a partir da URL (não permite alterar)
        entrevista.Id = id;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existe = await _repository.ExistsAsync(id);
        if (!existe)
        {
            return NotFound(new { message = "Entrevista não encontrada" });
        }

        // Validar regras de negócio (isUpdate = true para excluir a própria entrevista da verificação de conflito)
        var (isValid, errorMessage) = await _validationService.ValidateEntrevistaAsync(entrevista, isUpdate: true);
        if (!isValid)
        {
            return BadRequest(new { message = errorMessage });
        }

        var atualizada = await _repository.UpdateAsync(entrevista);
        return Ok(atualizada);
    }

    /// <summary>
    /// Cancela/remove uma entrevista
    /// </summary>
    /// <param name="id">ID da entrevista</param>
    /// <returns>Confirmação de remoção</returns>
    /// <response code="204">Entrevista removida</response>
    /// <response code="404">Entrevista não encontrada</response>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "🗑️ Cancelar/remover entrevista",
        Description = "Remove permanentemente uma entrevista do sistema"
    )]
    [SwaggerResponse(204, "Entrevista removida")]
    [SwaggerResponse(404, "Entrevista não encontrada")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existe = await _repository.ExistsAsync(id);
        if (!existe)
        {
            return NotFound(new { message = "Entrevista não encontrada" });
        }

        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
