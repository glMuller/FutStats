using System.Collections.Generic;
using System.Threading.Tasks;
using FutStatsAPI.Domain.Entities;

namespace FutStatsAPI.Domain.Interfaces
{
    public interface IJogadorRepository : IRepositoryBase<Jogador>
    {
        Task<(List<Jogador> Items, int TotalCount)> GetPagedByTimeAsync(int timeId, int pageNumber, int pageSize);
        Task<bool> ExisteNumeroNoTimeAsync(int timeId, int numero, int? ignorarJogadorId = null);
    }
}