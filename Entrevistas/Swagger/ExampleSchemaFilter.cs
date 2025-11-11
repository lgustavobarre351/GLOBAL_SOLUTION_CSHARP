using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using ProjetoEntrevistas.Models;

namespace ProjetoEntrevistas.Swagger;

/// <summary>
/// Filtro para customizar os exemplos exibidos no Swagger
/// </summary>
public class ExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(Entrevista))
        {
            // Exemplo completo e descritivo para Entrevista (POST)
            schema.Example = new OpenApiObject
            {
                ["idEmpregador"] = new OpenApiString("03439d2b-3e44-4f35-86d6-6df5d56dae15"),
                ["idCandidato"] = new OpenApiString("fee18a74-9237-4a17-88f3-1fd01f00d93e"),
                ["dataHora"] = new OpenApiString("2025-11-15T10:00:00Z"),
                ["duracaoMinutos"] = new OpenApiInteger(60),
                ["tipo"] = new OpenApiInteger(0),
                ["status"] = new OpenApiInteger(0),
                ["linkReuniao"] = new OpenApiString("https://meet.google.com/abc-defg-hij"),
                ["local"] = new OpenApiString("Av. Paulista, 1000 - São Paulo/SP"),
                ["observacoes"] = new OpenApiString("Entrevista técnica inicial")
            };
            
            // ✅ Descrições ULTRA detalhadas nos properties
            if (schema.Properties != null)
            {
                if (schema.Properties.TryGetValue("tipo", out var tipoProp))
                {
                    tipoProp.Description = "🎥 TIPO DE ENTREVISTA (use o número):\n• Digite 0 = online (videochamada - OBRIGATÓRIO linkReuniao)\n• Digite 1 = presencial (escritório - OBRIGATÓRIO local)\n• Digite 2 = telefone (ligação telefônica)";
                }
                if (schema.Properties.TryGetValue("status", out var statusProp))
                {
                    statusProp.Description = "📊 STATUS DA ENTREVISTA (use o número):\n• Digite 0 = scheduled/agendada (padrão inicial)\n• Digite 1 = canceled/cancelada\n• Digite 2 = completed/concluída";
                }
                if (schema.Properties.TryGetValue("duracaoMinutos", out var duracaoProp))
                {
                    duracaoProp.Description = "⏱️ Duração em minutos (mínimo: 15, máximo: 480)";
                }
                if (schema.Properties.TryGetValue("linkReuniao", out var linkProp))
                {
                    linkProp.Description = "🔗 Link da reunião online (OBRIGATÓRIO quando tipo = 0)";
                }
                if (schema.Properties.TryGetValue("local", out var localProp))
                {
                    localProp.Description = "📍 Endereço completo (OBRIGATÓRIO quando tipo = 1)";
                }
            }
        }
        else if (context.Type == typeof(Models.Candidato))
        {
            // Exemplo descritivo para Candidato
            schema.Example = new OpenApiObject
            {
                ["nome"] = new OpenApiString("João da Silva"),
                ["email"] = new OpenApiString("joao.silva@email.com"),
                ["telefone"] = new OpenApiString("11987654321")
            };
            
            // ✅ Descrições detalhadas
            if (schema.Properties != null)
            {
                if (schema.Properties.TryGetValue("telefone", out var telProp))
                {
                    telProp.Description = "📱 Telefone APENAS NÚMEROS (10 ou 11 dígitos) - Exemplo: '11987654321' - SEM parênteses, traços ou espaços!";
                }
            }
        }
        else if (context.Type == typeof(Models.Empregador))
        {
            // Exemplo descritivo para Empregador
            schema.Example = new OpenApiObject
            {
                ["nome"] = new OpenApiString("Tech Solutions RH"),
                ["email"] = new OpenApiString("rh@techsolutions.com"),
                ["telefone"] = new OpenApiString("1134567890")
            };
            
            // ✅ Descrições detalhadas
            if (schema.Properties != null)
            {
                if (schema.Properties.TryGetValue("telefone", out var telProp))
                {
                    telProp.Description = "📱 Telefone APENAS NÚMEROS (10 ou 11 dígitos) - Exemplo: '1134567890' - SEM parênteses, traços ou espaços!";
                }
            }
        }
        else if (context.Type == typeof(Models.DTOs.CriarEntrevistaDto))
        {
            // Exemplo COMPLETO para o DTO simplificado
            schema.Example = new OpenApiObject
            {
                ["idEmpregador"] = new OpenApiString("03439d2b-3e44-4f35-86d6-6df5d56dae15"),
                ["idCandidato"] = new OpenApiString("fee18a74-9237-4a17-88f3-1fd01f00d93e"),
                ["data"] = new OpenApiString("2025-11-15"),
                ["hora"] = new OpenApiString("10:00"),
                ["duracaoMinutos"] = new OpenApiInteger(60),
                ["tipo"] = new OpenApiInteger(0),
                ["linkReuniao"] = new OpenApiString("https://meet.google.com/abc-defg-hij"),
                ["local"] = new OpenApiString("Av. Paulista, 1000 - São Paulo/SP"),
                ["observacoes"] = new OpenApiString("Entrevista técnica")
            };
            
            // ✅ Descrições detalhadas
            if (schema.Properties != null)
            {
                if (schema.Properties.TryGetValue("tipo", out var tipoProp))
                {
                    tipoProp.Description = "🎥 TIPO DE ENTREVISTA (use o número):\n• Digite 0 = online (videochamada - OBRIGATÓRIO linkReuniao)\n• Digite 1 = presencial (escritório - OBRIGATÓRIO local)\n• Digite 2 = telefone (ligação telefônica)";
                }
                if (schema.Properties.TryGetValue("data", out var dataProp))
                {
                    dataProp.Description = "📅 Data no formato AAAA-MM-DD - Exemplo: '2025-11-15'";
                }
                if (schema.Properties.TryGetValue("hora", out var horaProp))
                {
                    horaProp.Description = "🕐 Hora no formato HH:MM - Exemplo: '10:00' ou '14:30'";
                }
                if (schema.Properties.TryGetValue("linkReuniao", out var linkProp))
                {
                    linkProp.Description = "🔗 Link da reunião online (OBRIGATÓRIO quando tipo = 0)";
                }
                if (schema.Properties.TryGetValue("local", out var localProp))
                {
                    localProp.Description = "📍 Endereço completo (OBRIGATÓRIO quando tipo = 1)";
                }
            }
        }
    }
}
