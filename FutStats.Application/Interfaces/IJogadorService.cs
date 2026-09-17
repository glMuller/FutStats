using System.Threading.Tasks;
using FutStatsAPI.Application.DTOs;
using FutStatsAPI.Domain.Common;

namespace FutStatsAPI.Application.Interfaces
{
    public interface IJogadorService
    {
        Task<PagedResult<JogadorDto>> ObterPaginadoPorTimeAsync(int timeId, int pageNumber, int pageSize);
        Task<JogadorDto> ObterPorIdAsync(int id);
        Task<JogadorDto> CriarAsync(CriarJogadorDto dto);
        Task AtualizarAsync(int id, AtualizarJogadorDto dto);
        Task RemoverAsync(int id);
    }
}