using ProjetoEntrevistas.Models;
using ProjetoEntrevistas.Models.Enums;
using ProjetoEntrevistas.Repositories;

namespace ProjetoEntrevistas.Services;

/// <summary>
/// Serviço de validação de regras de negócio para entrevistas
/// </summary>
public interface IEntrevistaValidationService
{
    Task<(bool IsValid, string ErrorMessage)> ValidateEntrevistaAsync(Entrevista entrevista, bool isUpdate = false);
}

public class EntrevistaValidationService : IEntrevistaValidationService
{
    private readonly IEntrevistaRepository _entrevistaRepository;
    private readonly IEmpregadorRepository _empregadorRepository;
    private readonly ICandidatoRepository _candidatoRepository;

    public EntrevistaValidationService(
        IEntrevistaRepository entrevistaRepository,
        IEmpregadorRepository empregadorRepository,
        ICandidatoRepository candidatoRepository)
    {
        _entrevistaRepository = entrevistaRepository;
        _empregadorRepository = empregadorRepository;
        _candidatoRepository = candidatoRepository;
    }

    public async Task<(bool IsValid, string ErrorMessage)> ValidateEntrevistaAsync(Entrevista entrevista, bool isUpdate = false)
    {
        // 1. ✅ VALIDAR ENUMS - Garantir que apenas valores válidos sejam aceitos
        if (!Enum.IsDefined(typeof(TipoEntrevista), entrevista.Tipo))
        {
            return (false, "Tipo inválido. Use: 0 (online), 1 (presencial) ou 2 (telefone)");
        }
        
        if (!Enum.IsDefined(typeof(StatusEntrevista), entrevista.Status))
        {
            return (false, "Status inválido. Use: 0 (scheduled), 1 (canceled) ou 2 (completed)");
        }

        // 2. Validar se empregador existe
        if (!await _empregadorRepository.ExistsAsync(entrevista.IdEmpregador))
        {
            return (false, "Empregador não encontrado");
        }

        // 3. Validar se candidato existe
        if (!await _candidatoRepository.ExistsAsync(entrevista.IdCandidato))
        {
            return (false, "Candidato não encontrado");
        }

        // 4. Validar data/hora não pode ser no passado
        if (entrevista.DataHora < DateTime.UtcNow.AddMinutes(-5)) // Tolerância de 5 minutos
        {
            return (false, "Não é possível agendar entrevistas no passado");
        }

        // 4. Validar duração mínima e máxima
        if (entrevista.DuracaoMinutos < 15)
        {
            return (false, "Duração mínima é de 15 minutos");
        }

        if (entrevista.DuracaoMinutos > 480)
        {
            return (false, "Duração máxima é de 480 minutos (8 horas)");
        }

        // 5. Validar campos obrigatórios por tipo
        switch (entrevista.Tipo)
        {
            case TipoEntrevista.online:
                if (string.IsNullOrWhiteSpace(entrevista.LinkReuniao))
                {
                    return (false, "Link de reunião é obrigatório para entrevistas online");
                }
                
                if (!Uri.TryCreate(entrevista.LinkReuniao, UriKind.Absolute, out var uri) || 
                    (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                {
                    return (false, "Link de reunião inválido. Deve ser uma URL válida (http/https)");
                }
                break;

            case TipoEntrevista.presencial:
                if (string.IsNullOrWhiteSpace(entrevista.Local))
                {
                    return (false, "Local é obrigatório para entrevistas presenciais");
                }
                break;

            case TipoEntrevista.telefone:
                // Telefone não requer campos adicionais obrigatórios
                break;
        }

        // 6. Verificar conflito de horários para o candidato
        var entrevistaIdToExclude = isUpdate ? entrevista.Id : (Guid?)null;
        var hasConflict = await _entrevistaRepository.HasConflictAsync(
            entrevista.IdCandidato, 
            entrevista.DataHora, 
            entrevista.DuracaoMinutos,
            entrevistaIdToExclude);

        if (hasConflict)
        {
            return (false, "Este candidato já possui uma entrevista agendada neste horário");
        }

        // 7. Validar horário comercial (opcional - pode comentar se não quiser)
        var hora = entrevista.DataHora.Hour;
        if (hora < 7 || hora >= 22)
        {
            return (false, "Entrevistas devem ser agendadas entre 07:00 e 22:00");
        }

        return (true, string.Empty);
    }
}
