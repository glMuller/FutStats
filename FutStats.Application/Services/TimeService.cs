using AutoMapper;
using FutStatsAPI.Application.DTOs;
using FutStatsAPI.Application.Exceptions;
using FutStatsAPI.Application.Interfaces;
using FutStatsAPI.Domain.Common;
using FutStatsAPI.Domain.Entities;
using FutStatsAPI.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FutStatsAPI.Application.Services
{
    public class TimeService : ITimeService
    {
        private readonly ITimeRepository _timeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TimeService> _logger;

        public TimeService(ITimeRepository timeRepository, IMapper mapper, ILogger<TimeService> logger)
        {
            _timeRepository = timeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<TimeDto>> ObterPaginadoAsync(int pageNumber, int pageSize)
        {
            // Garante valores mínimos sensatos mesmo se o cliente mandar algo inválido
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var (items, totalCount) = await _timeRepository.GetPagedAsync(pageNumber, pageSize);
            var dtos = _mapper.Map<List<TimeDto>>(items);

            _logger.LogInformation("Listagem de times: página {PageNumber}, tamanho {PageSize}, total {TotalCount}",
                pageNumber, pageSize, totalCount);

            return new PagedResult<TimeDto>(dtos, totalCount, pageNumber, pageSize);
        }

        public async Task<TimeDetalheDto> ObterPorIdAsync(int id)
        {
            var time = await _timeRepository.GetByIdComJogadoresAsync(id);
            if (time is null)
            {
                _logger.LogWarning("Time com Id {Id} não encontrado", id);
                throw new NotFoundException($"Time com Id {id} não encontrado.");
            }

            return _mapper.Map<TimeDetalheDto>(time);
        }

        public async Task<TimeDto> CriarAsync(CriarTimeDto dto)
        {
            var jaExiste = await _timeRepository.ExisteComNomeAsync(dto.Nome);
            if (jaExiste)
            {
                _logger.LogWarning("Tentativa de criar time duplicado: {Nome}", dto.Nome);
                throw new BusinessException($"Já existe um time cadastrado com o nome '{dto.Nome}'.");
            }

            var time = _mapper.Map<Time>(dto);
            await _timeRepository.AddAsync(time);
            await _timeRepository.SaveChangesAsync();

            _logger.LogInformation("Time criado com sucesso: {Nome} (Id {Id})", time.Nome, time.Id);

            return _mapper.Map<TimeDto>(time);
        }

        public async Task AtualizarAsync(int id, AtualizarTimeDto dto)
        {
            var time = await _timeRepository.GetByIdAsync(id);
            if (time is null)
                throw new NotFoundException($"Time com Id {id} não encontrado.");

            // Se o nome mudou, precisa checar duplicidade de novo
            if (!time.Nome.Equals(dto.Nome, System.StringComparison.OrdinalIgnoreCase))
            {
                var jaExiste = await _timeRepository.ExisteComNomeAsync(dto.Nome);
                if (jaExiste)
                    throw new BusinessException($"Já existe um time cadastrado com o nome '{dto.Nome}'.");
            }

            time.Nome = dto.Nome;
            time.Cidade = dto.Cidade;
            time.Estado = dto.Estado;
            time.AnoFundacao = dto.AnoFundacao;

            _timeRepository.Update(time);
            await _timeRepository.SaveChangesAsync();

            _logger.LogInformation("Time {Id} atualizado com sucesso", id);
        }

        public async Task RemoverAsync(int id)
        {
            var time = await _timeRepository.GetByIdAsync(id);
            if (time is null)
                throw new NotFoundException($"Time com Id {id} não encontrado.");

            _timeRepository.Remove(time);
            await _timeRepository.SaveChangesAsync();

            _logger.LogInformation("Time {Id} removido com sucesso", id);
        }
    }
}