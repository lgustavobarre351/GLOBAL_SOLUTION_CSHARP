using Microsoft.EntityFrameworkCore;
using ProjetoEntrevistas.Data;
using ProjetoEntrevistas.Models;

namespace ProjetoEntrevistas.Repositories;

public class EmpregadorRepository : IEmpregadorRepository
{
    private readonly AppDbContext _context;

    public EmpregadorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Empregador>> GetAllAsync()
    {
        return await _context.Empregadores
            .OrderBy(e => e.Nome)
            .ToListAsync();
    }

    public async Task<Empregador?> GetByIdAsync(Guid id)
    {
        return await _context.Empregadores
            .Include(e => e.Entrevistas)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Empregador?> GetByEmailAsync(string email)
    {
        return await _context.Empregadores
            .FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task<Empregador> CreateAsync(Empregador empregador)
    {
        empregador.Id = Guid.NewGuid();
        _context.Empregadores.Add(empregador);
        await _context.SaveChangesAsync();
        return empregador;
    }

    public async Task<Empregador> UpdateAsync(Empregador empregador)
    {
        _context.Empregadores.Update(empregador);
        await _context.SaveChangesAsync();
        return empregador;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var empregador = await _context.Empregadores.FindAsync(id);
        if (empregador == null) return false;

        _context.Empregadores.Remove(empregador);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Empregadores.AnyAsync(e => e.Id == id);
    }

    public async Task<bool> HasEntrevistasAsync(Guid id)
    {
        return await _context.Entrevistas
            .AnyAsync(e => e.IdEmpregador == id);
    }
}
