using Microsoft.EntityFrameworkCore;
using ProjetoEntrevistas.Data;
using ProjetoEntrevistas.Models;
using ProjetoEntrevistas.Models.Enums;

namespace ProjetoEntrevistas.Repositories;

public class EntrevistaRepository : IEntrevistaRepository
{
    private readonly AppDbContext _context;

    public EntrevistaRepository(AppDbContext context)
    {
        _context = context;
    }

    // LINQ 1: Listar todas com ordenação e includes
    public async Task<IEnumerable<Entrevista>> GetAllAsync()
    {
        return await _context.Entrevistas
            .Include(e => e.Empregador)
            .Include(e => e.Candidato)
            .OrderByDescending(e => e.DataHora)
            .ToListAsync();
    }

    public async Task<Entrevista?> GetByIdAsync(Guid id)
    {
        return await _context.Entrevistas
            .Include(e => e.Empregador)
            .Include(e => e.Candidato)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    // LINQ 2: Filtro por empregador com Where
    public async Task<IEnumerable<Entrevista>> GetByEmpregadorAsync(Guid empregadorId)
    {
        return await _context.Entrevistas
            .Include(e => e.Candidato)
            .Where(e => e.IdEmpregador == empregadorId)
            .OrderByDescending(e => e.DataHora)
            .ToListAsync();
    }

    // LINQ 3: Filtro por candidato com Where
    public async Task<IEnumerable<Entrevista>> GetByCandidatoAsync(Guid candidatoId)
    {
        return await _context.Entrevistas
            .Include(e => e.Empregador)
            .Where(e => e.IdCandidato == candidatoId)
            .OrderByDescending(e => e.DataHora)
            .ToListAsync();
    }

    // LINQ 4: Filtro por status
    public async Task<IEnumerable<Entrevista>> GetByStatusAsync(StatusEntrevista status)
    {
        return await _context.Entrevistas
            .Include(e => e.Empregador)
            .Include(e => e.Candidato)
            .Where(e => e.Status == status)
            .OrderByDescending(e => e.DataHora)
            .ToListAsync();
    }

    // LINQ 5: Filtro por tipo
    public async Task<IEnumerable<Entrevista>> GetByTipoAsync(TipoEntrevista tipo)
    {
        return await _context.Entrevistas
            .Include(e => e.Empregador)
            .Include(e => e.Candidato)
            .Where(e => e.Tipo == tipo)
            .OrderByDescending(e => e.DataHora)
            .ToListAsync();
    }

    // LINQ 6: Filtro por data específica
    public async Task<IEnumerable<Entrevista>> GetByDataAsync(DateTime data)
    {
        var inicioDia = data.Date;
        var fimDia = inicioDia.AddDays(1);

        return await _context.Entrevistas
            .Include(e => e.Empregador)
            .Include(e => e.Candidato)
            .Where(e => e.DataHora >= inicioDia && e.DataHora < fimDia)
            .OrderBy(e => e.DataHora)
            .ToListAsync();
    }

    public async Task<Entrevista> CreateAsync(Entrevista entrevista)
    {
        entrevista.Id = Guid.NewGuid();
        entrevista.CriadoEm = DateTime.UtcNow;
        _context.Entrevistas.Add(entrevista);
        await _context.SaveChangesAsync();
        
        // Recarregar com navegação
        return (await GetByIdAsync(entrevista.Id))!;
    }

    public async Task<Entrevista> UpdateAsync(Entrevista entrevista)
    {
        _context.Entrevistas.Update(entrevista);
        await _context.SaveChangesAsync();
        
        // Recarregar com navegação
        return (await GetByIdAsync(entrevista.Id))!;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entrevista = await _context.Entrevistas.FindAsync(id);
        if (entrevista == null) return false;

        _context.Entrevistas.Remove(entrevista);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Entrevistas.AnyAsync(e => e.Id == id);
    }

    // LINQ 7: Verificar conflitos de horário
    public async Task<bool> HasConflictAsync(Guid candidatoId, DateTime dataHora, int duracaoMinutos, Guid? entrevistaIdToExclude = null)
    {
        var fimEntrevista = dataHora.AddMinutes(duracaoMinutos);

        var query = _context.Entrevistas
            .Where(e => e.IdCandidato == candidatoId)
            .Where(e => e.Status == StatusEntrevista.scheduled)
            .Where(e => 
                (e.DataHora < fimEntrevista) && 
                (e.DataHora.AddMinutes(e.DuracaoMinutos) > dataHora)
            );

        if (entrevistaIdToExclude.HasValue)
        {
            query = query.Where(e => e.Id != entrevistaIdToExclude.Value);
        }

        return await query.AnyAsync();
    }

    // LINQ 8: Dashboard com agregações (GroupBy, Count, Sum, Average)
    public async Task<Dictionary<string, object>> GetDashboardAsync()
    {
        var totalEntrevistas = await _context.Entrevistas.CountAsync();
        
        // Agrupamento por status
        var porStatus = await _context.Entrevistas
            .GroupBy(e => e.Status)
            .Select(g => new
            {
                Status = g.Key.ToString(),
                Quantidade = g.Count()
            })
            .ToListAsync();

        // Agrupamento por tipo
        var porTipo = await _context.Entrevistas
            .GroupBy(e => e.Tipo)
            .Select(g => new
            {
                Tipo = g.Key.ToString(),
                Quantidade = g.Count(),
                DuracaoMedia = g.Average(e => e.DuracaoMinutos)
            })
            .ToListAsync();

        // Próximas entrevistas (agendadas)
        var proximasEntrevistas = await _context.Entrevistas
            .Where(e => e.Status == StatusEntrevista.scheduled && e.DataHora > DateTime.UtcNow)
            .OrderBy(e => e.DataHora)
            .Take(5)
            .Select(e => new
            {
                e.Id,
                e.DataHora,
                e.Tipo,
                e.DuracaoMinutos
            })
            .ToListAsync();

        return new Dictionary<string, object>
        {
            { "TotalEntrevistas", totalEntrevistas },
            { "PorStatus", porStatus },
            { "PorTipo", porTipo },
            { "ProximasEntrevistas", proximasEntrevistas }
        };
    }
}
