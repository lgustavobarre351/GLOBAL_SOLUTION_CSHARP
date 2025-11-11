using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjetoEntrevistas.Models.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjetoEntrevistas.Models;

/// <summary>
/// Representa uma entrevista agendada no sistema
/// </summary>
[Table("entrevistas")]
public class Entrevista
{
    /// <summary>
    /// ID único da entrevista
    /// </summary>
    [Key]
    [Column("id")]
    [SwaggerSchema(ReadOnly = true, Description = "ID único da entrevista (gerado automaticamente)")]
    public Guid Id { get; set; }

    /// <summary>
    /// ID do empregador que agendou a entrevista
    /// </summary>
    [Required(ErrorMessage = "Empregador é obrigatório")]
    [Column("employer_id")]
    [SwaggerSchema(Description = "📋 UUID do empregador (copie de GET /api/v1/empregadores) - Exemplo: '03439d2b-3e44-4f35-86d6-6df5d56dae15'")]
    public Guid IdEmpregador { get; set; }

    /// <summary>
    /// ID do candidato que será entrevistado
    /// </summary>
    [Required(ErrorMessage = "Candidato é obrigatório")]
    [Column("candidate_id")]
    [SwaggerSchema(Description = "👤 UUID do candidato (copie de GET /api/v1/candidatos) - Exemplo: 'fee18a74-9237-4a17-88f3-1fd01f00d93e'")]
    public Guid IdCandidato { get; set; }

    /// <summary>
    /// Data e hora da entrevista
    /// </summary>
    [Required(ErrorMessage = "Data e hora são obrigatórias")]
    [Column("starts_at")]
    [SwaggerSchema(Description = "📅 Data e hora no formato ISO 8601 - Exemplo: '2025-11-15T10:00:00Z' (Z = UTC)")]
    public DateTime DataHora { get; set; }

    /// <summary>
    /// Duração da entrevista em minutos
    /// </summary>
    [Required(ErrorMessage = "Duração é obrigatória")]
    [Range(1, 480, ErrorMessage = "Duração deve ser entre 1 e 480 minutos (8 horas)")]
    [Column("duration_minutes")]
    [SwaggerSchema(Description = "⏱️ Duração em minutos (mínimo: 15, máximo: 480) - Exemplo: 60")]
    public int DuracaoMinutos { get; set; } = 60;

    /// <summary>
    /// Tipo da entrevista (online, presencial, telefone)
    /// </summary>
    [Required(ErrorMessage = "Tipo é obrigatório")]
    [EnumDataType(typeof(TipoEntrevista), ErrorMessage = "Tipo inválido. Use: 0 (online), 1 (presencial) ou 2 (telefone)")]
    [Column("type")]
    [SwaggerSchema(Description = "🎥 Tipo de entrevista: 0 = online (videochamada), 1 = presencial (escritório), 2 = telefone - Exemplo: 0")]
    public TipoEntrevista Tipo { get; set; }

    /// <summary>
    /// Status da entrevista (agendada, cancelada, concluida)
    /// </summary>
    [Required(ErrorMessage = "Status é obrigatório")]
    [EnumDataType(typeof(StatusEntrevista), ErrorMessage = "Status inválido. Use: 0 (scheduled), 1 (canceled) ou 2 (completed)")]
    [Column("status")]
    [SwaggerSchema(Description = "📊 Status: 0 = scheduled (agendada), 1 = canceled (cancelada), 2 = completed (concluída) - Exemplo: 0")]
    public StatusEntrevista Status { get; set; } = StatusEntrevista.scheduled;

    /// <summary>
    /// Link da reunião online (obrigatório se tipo = online)
    /// </summary>
    [Url(ErrorMessage = "Link de reunião inválido")]
    [StringLength(500, ErrorMessage = "Link deve ter no máximo 500 caracteres")]
    [Column("meeting_link")]
    [SwaggerSchema(Description = "🔗 Link da videochamada (OBRIGATÓRIO se tipo = 0) - Exemplo: 'https://meet.google.com/abc-defg-hij'")]
    public string? LinkReuniao { get; set; }

    /// <summary>
    /// Local da entrevista presencial (obrigatório se tipo = presencial)
    /// </summary>
    [StringLength(500, ErrorMessage = "Local deve ter no máximo 500 caracteres")]
    [Column("location")]
    [SwaggerSchema(Description = "📍 Endereço completo (OBRIGATÓRIO se tipo = 1) - Exemplo: 'Av. Paulista, 1000 - São Paulo/SP'")]
    public string? Local { get; set; }

    /// <summary>
    /// Observações adicionais sobre a entrevista
    /// </summary>
    [StringLength(1000, ErrorMessage = "Observações devem ter no máximo 1000 caracteres")]
    [Column("notes")]
    [SwaggerSchema(Description = "📝 Observações, notas ou instruções adicionais - Exemplo: 'Trazer portfólio impresso'")]
    public string? Observacoes { get; set; }

    /// <summary>
    /// Data de criação do registro
    /// </summary>
    [Column("created_at")]
    [SwaggerSchema(ReadOnly = true, Description = "Data de criação (gerada automaticamente)")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Navegação
    /// <summary>
    /// Empregador que agendou esta entrevista
    /// </summary>
    [ForeignKey(nameof(IdEmpregador))]
    [SwaggerSchema(ReadOnly = true, Description = "Dados do empregador (não enviar no POST/PUT)")]
    public Empregador? Empregador { get; set; }

    /// <summary>
    /// Candidato que será entrevistado
    /// </summary>
    [ForeignKey(nameof(IdCandidato))]
    [SwaggerSchema(ReadOnly = true, Description = "Dados do candidato (não enviar no POST/PUT)")]
    public Candidato? Candidato { get; set; }
}
