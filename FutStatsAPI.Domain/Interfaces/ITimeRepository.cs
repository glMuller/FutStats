using System.Collections.Generic;
using System.Threading.Tasks;
using FutStatsAPI.Domain.Entities;

namespace FutStatsAPI.Domain.Interfaces
{
    public interface ITimeRepository : IRepositoryBase<Time>
    {
        Task<(List<Time> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
        Task<Time?> GetByIdComJogadoresAsync(int id);
        Task<bool> ExisteComNomeAsync(string nome);
    }
}