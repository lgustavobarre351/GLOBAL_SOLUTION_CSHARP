using ProjetoEntrevistas.Models;

namespace ProjetoEntrevistas.Repositories;

public interface ICandidatoRepository
{
    Task<IEnumerable<Candidato>> GetAllAsync();
    Task<Candidato?> GetByIdAsync(Guid id);
    Task<Candidato?> GetByEmailAsync(string email);
    Task<Candidato> CreateAsync(Candidato candidato);
    Task<Candidato> UpdateAsync(Candidato candidato);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> HasEntrevistasAsync(Guid id);
}
