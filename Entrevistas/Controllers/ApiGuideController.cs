using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjetoEntrevistas.Controllers;

/// <summary>
/// 📖 GUIA DE USO DA API
/// </summary>
[ApiController]
[Route("api/v1/guide")]
public class ApiGuideController : ControllerBase
{
    /// <summary>
    /// 🚀 GUIA COMPLETO - Como usar a API de Entrevistas
    /// </summary>
    /// <returns>Instruções de uso da API</returns>
    [HttpGet("como-usar")]
    [SwaggerOperation(
        Summary = "🚀 COMO USAR A API - Guia completo passo a passo",
        Description = "Guia detalhado com todos os endpoints, exemplos práticos e valores válidos para criar entrevistas"
    )]
    [SwaggerResponse(200, "Guia de uso completo")]
    public ActionResult GetGuide()
    {
        var guide = new
        {
            Titulo = "🎯 API de Agendamento de Entrevistas - GUIA COMPLETO",
            
            PassoAPasso = new
            {
                Passo1 = "📝 Criar empregadores: POST /api/v1/empregadores",
                Passo2 = "👤 Criar candidatos: POST /api/v1/candidatos",
                Passo3 = "🔍 Listar IDs: GET /api/v1/empregadores e GET /api/v1/candidatos",
                Passo4 = "📅 Agendar entrevista: POST /api/v1/entrevistas (use os IDs obtidos)"
            },
            
            PADROES_DE_CAMPOS = new
            {
                Nome = "Texto livre - Exemplo: 'João da Silva' ou 'Tech Solutions RH'",
                Email = "Formato email válido - Exemplo: 'joao@email.com' (único no sistema)",
                Telefone = "APENAS NÚMEROS (10 ou 11 dígitos) - Exemplo: '11987654321' ou '1134567890'",
                DataHora = "ISO 8601 - Exemplo: '2025-11-15T10:00:00Z' (ano-mês-diaThora:minuto:segundoZ)",
                LinkReuniao = "URL completa - Exemplo: 'https://meet.google.com/abc-defg-hij'",
                Local = "Texto livre - Exemplo: 'Av. Paulista, 1000 - São Paulo/SP'",
                DuracaoMinutos = "Número entre 15 e 480 - Exemplo: 60",
                Tipo = "NÚMERO 0, 1 ou 2 (sem aspas)",
                Status = "NÚMERO 0, 1 ou 2 (sem aspas)"
            },
            
            ATENCAO_IMPORTANTE = new
            {
                CampoTipo = "Digite apenas o NÚMERO (0, 1 ou 2) - NÃO digite texto!",
                CampoStatus = "Digite apenas o NÚMERO (0, 1 ou 2) - NÃO digite texto!",
                CampoTelefone = "Telefone deve ter APENAS NÚMEROS - SEM parênteses, traços ou espaços! Ex: '11987654321'"
            },
            
            TiposDisponiveis = new
            {
                _0_Online = "Digite 0 para videochamada (OBRIGATÓRIO preencher linkReuniao)",
                _1_Presencial = "Digite 1 para escritório (OBRIGATÓRIO preencher local)",
                _2_Telefone = "Digite 2 para ligação telefônica"
            },
            
            StatusDisponiveis = new
            {
                _0_Scheduled = "Digite 0 para agendada (padrão para novas entrevistas)",
                _1_Canceled = "Digite 1 para cancelada",
                _2_Completed = "Digite 2 para concluída"
            },
            
            ExemplosCompletos = new
            {
                EntrevistaOnline = new
                {
                    Descricao = "✅ Tipo = 0 (online) - OBRIGATÓRIO linkReuniao",
                    Exemplo = new
                    {
                        IdEmpregador = "03439d2b-3e44-4f35-86d6-6df5d56dae15",
                        IdCandidato = "fee18a74-9237-4a17-88f3-1fd01f00d93e",
                        DataHora = "2025-11-15T10:00:00Z",
                        DuracaoMinutos = 60,
                        Tipo = 0,
                        Status = 0,
                        LinkReuniao = "https://meet.google.com/abc-defg-hij",
                        Observacoes = "Entrevista técnica inicial"
                    }
                },
                EntrevistaPresencial = new
                {
                    Descricao = "✅ Tipo = 1 (presencial) - OBRIGATÓRIO local",
                    Exemplo = new
                    {
                        IdEmpregador = "03439d2b-3e44-4f35-86d6-6df5d56dae15",
                        IdCandidato = "fee18a74-9237-4a17-88f3-1fd01f00d93e",
                        DataHora = "2025-11-16T14:30:00Z",
                        DuracaoMinutos = 45,
                        Tipo = 1,
                        Status = 0,
                        Local = "Av. Paulista, 1000 - São Paulo/SP",
                        Observacoes = "Trazer documentos"
                    }
                },
                EntrevistaTelefone = new
                {
                    Descricao = "✅ Tipo = 2 (telefone) - Apenas campos básicos",
                    Exemplo = new
                    {
                        IdEmpregador = "03439d2b-3e44-4f35-86d6-6df5d56dae15",
                        IdCandidato = "fee18a74-9237-4a17-88f3-1fd01f00d93e",
                        DataHora = "2025-11-17T09:00:00Z",
                        DuracaoMinutos = 30,
                        Tipo = 2,
                        Status = 0,
                        Observacoes = "Ligar no número cadastrado"
                    }
                }
            },
            
            Endpoints = new
            {
                Empregadores = new
                {
                    Criar = "POST /api/v1/empregadores",
                    Listar = "GET /api/v1/empregadores",
                    BuscarPorId = "GET /api/v1/empregadores/{id}",
                    Atualizar = "PUT /api/v1/empregadores/{id}",
                    Deletar = "DELETE /api/v1/empregadores/{id}"
                },
                Candidatos = new
                {
                    Criar = "POST /api/v1/candidatos",
                    Listar = "GET /api/v1/candidatos",
                    BuscarPorId = "GET /api/v1/candidatos/{id}",
                    Atualizar = "PUT /api/v1/candidatos/{id}",
                    Deletar = "DELETE /api/v1/candidatos/{id}"
                },
                Entrevistas = new
                {
                    Criar = "POST /api/v1/entrevistas",
                    Listar = "GET /api/v1/entrevistas",
                    BuscarPorId = "GET /api/v1/entrevistas/{id}",
                    PorEmpregador = "GET /api/v1/entrevistas/empregador/{id}",
                    PorCandidato = "GET /api/v1/entrevistas/candidato/{id}",
                    PorStatus = "GET /api/v1/entrevistas/status/{status}",
                    PorTipo = "GET /api/v1/entrevistas/tipo/{tipo}",
                    AgendaDoDia = "GET /api/v1/entrevistas/agenda/{data}",
                    Dashboard = "GET /api/v1/entrevistas/dashboard",
                    Atualizar = "PUT /api/v1/entrevistas/{id}",
                    Cancelar = "DELETE /api/v1/entrevistas/{id}"
                }
            },
            
            Validacoes = new[]
            {
                "✅ Empregador e candidato devem existir no banco",
                "✅ Data/hora não pode ser no passado",
                "✅ Duração entre 15 e 480 minutos",
                "✅ LinkReuniao obrigatório quando tipo = 0 (online)",
                "✅ Local obrigatório quando tipo = 1 (presencial)",
                "✅ Verifica conflitos de horário do candidato",
                "✅ Horário comercial válido (07:00 - 22:00)"
            },
            
            DicaImportante = "💡 DICA: Use GET /api/v1/empregadores e GET /api/v1/candidatos para copiar os IDs reais antes de criar entrevistas!"
        };
        
        return Ok(guide);
    }
}
