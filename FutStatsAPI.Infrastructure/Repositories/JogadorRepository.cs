using FutStatsAPI.Domain.Entities;
using FutStatsAPI.Domain.Interfaces;
using FutStatsAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FutStatsAPI.Infrastructure.Repositories
{
    public class JogadorRepository : RepositoryBase<Jogador>, IJogadorRepository
    {
        public JogadorRepository(AppDbContext context) : base(context) { }

        public async Task<(List<Jogador> Items, int TotalCount)> GetPagedByTimeAsync(int timeId, int pageNumber, int pageSize)
        {
            var query = _context.Jogadores
                .Include(j => j.Time)
                .Where(j => j.TimeId == timeId)
                .OrderBy(j => j.Numero);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> ExisteNumeroNoTimeAsync(int timeId, int numero, int? ignorarJogadorId = null)
        {
            var query = _context.Jogadores
                .Where(j => j.TimeId == timeId && j.Numero == numero);

            if (ignorarJogadorId.HasValue)
                query = query.Where(j => j.Id != ignorarJogadorId.Value);

            return await query.AnyAsync();
        }
    }
}