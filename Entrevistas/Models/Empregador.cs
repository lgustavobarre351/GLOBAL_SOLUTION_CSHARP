using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjetoEntrevistas.Models;

/// <summary>
/// Representa um empregador/recrutador no sistema
/// </summary>
[Table("empregadores")]
public class Empregador
{
    /// <summary>
    /// ID único do empregador
    /// </summary>
    [Key]
    [Column("id")]
    [SwaggerSchema(ReadOnly = true, Description = "ID único do empregador (gerado automaticamente)")]
    public Guid Id { get; set; }

    /// <summary>
    /// Nome completo do empregador
    /// </summary>
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    [Column("name")]
    [SwaggerSchema(Description = "👔 Nome completo ou empresa - Exemplo: 'Maria Santos' ou 'Tech Solutions RH'")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Email do empregador (único)
    /// </summary>
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres")]
    [Column("email")]
    [SwaggerSchema(Description = "📧 Email válido (deve ser único) - Exemplo: 'rh@empresa.com'")]
    public string? Email { get; set; }

    /// <summary>
    /// Telefone de contato
    /// </summary>
    [Phone(ErrorMessage = "Telefone inválido")]
    [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Telefone deve conter apenas 10 ou 11 dígitos (ex: 1134567890)")]
    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    [Column("phone")]
    [SwaggerSchema(Description = "📱 Telefone APENAS NÚMEROS (10 ou 11 dígitos) - Exemplo: '1134567890'")]
    public string? Telefone { get; set; }

    /// <summary>
    /// Entrevistas agendadas por este empregador
    /// </summary>
    [SwaggerSchema(ReadOnly = true, Description = "Lista de entrevistas (não enviar no POST/PUT)")]
    public ICollection<Entrevista> Entrevistas { get; set; } = new List<Entrevista>();
}
