using System.Threading.Tasks;
using FutStatsAPI.Application.DTOs;
using FutStatsAPI.Domain.Common;

namespace FutStatsAPI.Application.Interfaces
{
    public interface ITimeService
    {
        Task<PagedResult<TimeDto>> ObterPaginadoAsync(int pageNumber, int pageSize);
        Task<TimeDetalheDto> ObterPorIdAsync(int id);
        Task<TimeDto> CriarAsync(CriarTimeDto dto);
        Task AtualizarAsync(int id, AtualizarTimeDto dto);
        Task RemoverAsync(int id);
    }
}