using System.ComponentModel.DataAnnotations;
using ProjetoEntrevistas.Models.Enums;

namespace ProjetoEntrevistas.Models.DTOs;

/// <summary>
/// DTO simplificado para criar entrevista com formato de data mais fácil
/// </summary>
public class CriarEntrevistaDto
{
    /// <summary>
    /// ID do empregador
    /// </summary>
    [Required(ErrorMessage = "ID do empregador é obrigatório")]
    public Guid IdEmpregador { get; set; }

    /// <summary>
    /// ID do candidato
    /// </summary>
    [Required(ErrorMessage = "ID do candidato é obrigatório")]
    public Guid IdCandidato { get; set; }

    /// <summary>
    /// Data da entrevista no formato simples: "2025-11-15" ou "2025-11-15 10:00"
    /// </summary>
    [Required(ErrorMessage = "Data é obrigatória")]
    public string Data { get; set; } = string.Empty;

    /// <summary>
    /// Hora da entrevista no formato "HH:mm" (ex: "10:00", "14:30")
    /// Opcional se a hora já estiver incluída no campo Data
    /// </summary>
    public string? Hora { get; set; }

    /// <summary>
    /// Duração em minutos (padrão: 60)
    /// </summary>
    [Range(15, 480, ErrorMessage = "Duração deve ser entre 15 e 480 minutos")]
    public int DuracaoMinutos { get; set; } = 60;

    /// <summary>
    /// Tipo da entrevista: 0, 1 ou 2
    /// </summary>
    [Required(ErrorMessage = "Tipo é obrigatório")]
    public int Tipo { get; set; }

    /// <summary>
    /// Link da reunião (obrigatório se tipo = 0)
    /// </summary>
    [Url(ErrorMessage = "Link inválido")]
    public string? LinkReuniao { get; set; }

    /// <summary>
    /// Local da entrevista (obrigatório se tipo = Presencial)
    /// </summary>
    public string? Local { get; set; }

    /// <summary>
    /// Observações
    /// </summary>
    public string? Observacoes { get; set; }

    /// <summary>
    /// Converte o DTO para o modelo Entrevista
    /// </summary>
    public Entrevista ToEntrevista()
    {
        // Parse da data
        DateTime dataHora;
        
        // Tentar parse com hora inclusa no campo Data
        if (DateTime.TryParse(Data + (Hora != null ? " " + Hora : ""), out dataHora))
        {
            // OK
        }
        // Tentar apenas a data e adicionar hora separada
        else if (DateTime.TryParse(Data, out var dataOnly))
        {
            if (!string.IsNullOrWhiteSpace(Hora) && TimeSpan.TryParse(Hora, out var hora))
            {
                dataHora = dataOnly.Add(hora);
            }
            else
            {
                dataHora = dataOnly.AddHours(9); // Default 09:00
            }
        }
        else
        {
            throw new ArgumentException("Formato de data inválido. Use 'YYYY-MM-DD' ou 'YYYY-MM-DD HH:mm'");
        }

        // Validar tipo (deve ser 0, 1 ou 2)
        if (!Enum.IsDefined(typeof(TipoEntrevista), Tipo))
        {
            throw new ArgumentException("Tipo inválido. Use: 0 (online), 1 (presencial) ou 2 (telefone)");
        }

        return new Entrevista
        {
            IdEmpregador = IdEmpregador,
            IdCandidato = IdCandidato,
            DataHora = dataHora,
            DuracaoMinutos = DuracaoMinutos,
            Tipo = (TipoEntrevista)Tipo,
            Status = StatusEntrevista.scheduled,
            LinkReuniao = LinkReuniao,
            Local = Local,
            Observacoes = Observacoes
        };
    }
}