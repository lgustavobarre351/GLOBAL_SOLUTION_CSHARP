using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjetoEntrevistas.Models;

/// <summary>
/// Representa um candidato no sistema
/// </summary>
[Table("candidatos")]
public class Candidato
{
    /// <summary>
    /// ID único do candidato
    /// </summary>
    [Key]
    [Column("id")]
    [SwaggerSchema(ReadOnly = true, Description = "ID único do candidato (gerado automaticamente)")]
    public Guid Id { get; set; }

    /// <summary>
    /// Nome completo do candidato
    /// </summary>
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    [Column("name")]
    [SwaggerSchema(Description = "👤 Nome completo - Exemplo: 'João da Silva'")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Email do candidato (único)
    /// </summary>
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres")]
    [Column("email")]
    [SwaggerSchema(Description = "📧 Email válido (deve ser único) - Exemplo: 'joao.silva@email.com'")]
    public string? Email { get; set; }

    /// <summary>
    /// Telefone de contato
    /// </summary>
    [Phone(ErrorMessage = "Telefone inválido")]
    [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Telefone deve conter apenas 10 ou 11 dígitos (ex: 11987654321)")]
    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    [Column("phone")]
    [SwaggerSchema(Description = "📱 Telefone APENAS NÚMEROS (10 ou 11 dígitos) - Exemplo: '11987654321'")]
    public string? Telefone { get; set; }

    /// <summary>
    /// Entrevistas agendadas para este candidato
    /// </summary>
    [SwaggerSchema(ReadOnly = true, Description = "Lista de entrevistas (não enviar no POST/PUT)")]
    public ICollection<Entrevista> Entrevistas { get; set; } = new List<Entrevista>();
}
