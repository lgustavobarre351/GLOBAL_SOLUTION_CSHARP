namespace ProjetoEntrevistas.Models.Enums;

/// <summary>
/// Status da entrevista (valores em inglês conforme banco de dados)
/// </summary>
public enum StatusEntrevista
{
    /// <summary>
    /// Entrevista agendada (status inicial)
    /// </summary>
    scheduled = 0,

    /// <summary>
    /// Entrevista cancelada
    /// </summary>
    canceled = 1,

    /// <summary>
    /// Entrevista concluída
    /// </summary>
    completed = 2
}
