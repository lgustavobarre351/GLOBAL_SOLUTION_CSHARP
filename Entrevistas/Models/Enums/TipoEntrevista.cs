namespace ProjetoEntrevistas.Models.Enums;

/// <summary>
/// Tipo de entrevista (valores em inglês conforme banco de dados)
/// </summary>
public enum TipoEntrevista
{
    /// <summary>
    /// Entrevista online (requer link de reunião)
    /// </summary>
    online = 0,

    /// <summary>
    /// Entrevista presencial (requer local)
    /// </summary>
    presencial = 1,

    /// <summary>
    /// Entrevista por telefone
    /// </summary>
    telefone = 2
}
