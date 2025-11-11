using ProjetoEntrevistas.Models;
using ProjetoEntrevistas.Models.Enums;

namespace ProjetoEntrevistas.Repositories;

public interface IEntrevistaRepository
{
    Task<IEnumerable<Entrevista>> GetAllAsync();
    Task<Entrevista?> GetByIdAsync(Guid id);
    Task<IEnumerable<Entrevista>> GetByEmpregadorAsync(Guid empregadorId);
    Task<IEnumerable<Entrevista>> GetByCandidatoAsync(Guid candidatoId);
    Task<IEnumerable<Entrevista>> GetByStatusAsync(StatusEntrevista status);
    Task<IEnumerable<Entrevista>> GetByTipoAsync(TipoEntrevista tipo);
    Task<IEnumerable<Entrevista>> GetByDataAsync(DateTime data);
    Task<Entrevista> CreateAsync(Entrevista entrevista);
    Task<Entrevista> UpdateAsync(Entrevista entrevista);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> HasConflictAsync(Guid candidatoId, DateTime dataHora, int duracaoMinutos, Guid? entrevistaIdToExclude = null);
    Task<Dictionary<string, object>> GetDashboardAsync();
}
