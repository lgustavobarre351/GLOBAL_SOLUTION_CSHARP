using ProjetoEntrevistas.Models;

namespace ProjetoEntrevistas.Repositories;

public interface IEmpregadorRepository
{
    Task<IEnumerable<Empregador>> GetAllAsync();
    Task<Empregador?> GetByIdAsync(Guid id);
    Task<Empregador?> GetByEmailAsync(string email);
    Task<Empregador> CreateAsync(Empregador empregador);
    Task<Empregador> UpdateAsync(Empregador empregador);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> HasEntrevistasAsync(Guid id);
}
