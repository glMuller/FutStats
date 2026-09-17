using FutStatsAPI.Domain.Entities;
using FutStatsAPI.Domain.Interfaces;
using FutStatsAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutStatsAPI.Infrastructure.Repositories
{
    public class TimeRepository : RepositoryBase<Time>, ITimeRepository
    {
        public TimeRepository(AppDbContext context) : base(context) { }

        public async Task<(List<Time> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Times
                .Include(t => t.Jogadores)   // necessário pra calcular QuantidadeJogadores no DTO
                .OrderBy(t => t.Nome);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Time?> GetByIdComJogadoresAsync(int id)
        {
            return await _context.Times
                .Include(t => t.Jogadores)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> ExisteComNomeAsync(string nome)
        {
            return await _context.Times
                .AnyAsync(t => t.Nome.ToLower() == nome.ToLower());
        }
    }
}