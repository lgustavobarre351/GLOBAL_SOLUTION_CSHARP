using Microsoft.EntityFrameworkCore;
using ProjetoEntrevistas.Data;
using ProjetoEntrevistas.Models;

namespace ProjetoEntrevistas.Repositories;

public class CandidatoRepository : ICandidatoRepository
{
    private readonly AppDbContext _context;

    public CandidatoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Candidato>> GetAllAsync()
    {
        return await _context.Candidatos
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<Candidato?> GetByIdAsync(Guid id)
    {
        return await _context.Candidatos
            .Include(c => c.Entrevistas)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Candidato?> GetByEmailAsync(string email)
    {
        return await _context.Candidatos
            .FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<Candidato> CreateAsync(Candidato candidato)
    {
        candidato.Id = Guid.NewGuid();
        _context.Candidatos.Add(candidato);
        await _context.SaveChangesAsync();
        return candidato;
    }

    public async Task<Candidato> UpdateAsync(Candidato candidato)
    {
        _context.Candidatos.Update(candidato);
        await _context.SaveChangesAsync();
        return candidato;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var candidato = await _context.Candidatos.FindAsync(id);
        if (candidato == null) return false;

        _context.Candidatos.Remove(candidato);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Candidatos.AnyAsync(c => c.Id == id);
    }

    public async Task<bool> HasEntrevistasAsync(Guid id)
    {
        return await _context.Entrevistas
            .AnyAsync(e => e.IdCandidato == id);
    }
}
