using AutoMapper;
using FutStatsAPI.Application.DTOs;
using FutStatsAPI.Application.Exceptions;
using FutStatsAPI.Application.Interfaces;
using FutStatsAPI.Domain.Common;
using FutStatsAPI.Domain.Entities;
using FutStatsAPI.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FutStatsAPI.Application.Services
{
    public class JogadorService : IJogadorService
    {
        private readonly IJogadorRepository _jogadorRepository;
        private readonly ITimeRepository _timeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<JogadorService> _logger;

        public JogadorService(
            IJogadorRepository jogadorRepository,
            ITimeRepository timeRepository,
            IMapper mapper,
            ILogger<JogadorService> logger)
        {
            _jogadorRepository = jogadorRepository;
            _timeRepository = timeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<JogadorDto>> ObterPaginadoPorTimeAsync(int timeId, int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var timeExiste = await _timeRepository.GetByIdAsync(timeId);
            if (timeExiste is null)
                throw new NotFoundException($"Time com Id {timeId} não encontrado.");

            var (items, totalCount) = await _jogadorRepository.GetPagedByTimeAsync(timeId, pageNumber, pageSize);
            var dtos = _mapper.Map<List<JogadorDto>>(items);

            _logger.LogInformation("Listagem de jogadores do time {TimeId}: página {PageNumber}, total {TotalCount}",
                timeId, pageNumber, totalCount);

            return new PagedResult<JogadorDto>(dtos, totalCount, pageNumber, pageSize);
        }

        public async Task<JogadorDto> ObterPorIdAsync(int id)
        {
            var jogador = await _jogadorRepository.GetByIdAsync(id);
            if (jogador is null)
                throw new NotFoundException($"Jogador com Id {id} não encontrado.");

            return _mapper.Map<JogadorDto>(jogador);
        }

        public async Task<JogadorDto> CriarAsync(CriarJogadorDto dto)
        {
            var time = await _timeRepository.GetByIdAsync(dto.TimeId);
            if (time is null)
                throw new NotFoundException($"Time com Id {dto.TimeId} não encontrado.");

            var posicao = ConverterPosicao(dto.Posicao);

            var numeroEmUso = await _jogadorRepository.ExisteNumeroNoTimeAsync(dto.TimeId, dto.Numero);
            if (numeroEmUso)
                throw new BusinessException($"O número {dto.Numero} já está em uso por outro jogador nesse time.");

            var jogador = new Jogador
            {
                Nome = dto.Nome,
                Numero = dto.Numero,
                Posicao = posicao,
                DataNascimento = dto.DataNascimento,
                TimeId = dto.TimeId
            };

            await _jogadorRepository.AddAsync(jogador);
            await _jogadorRepository.SaveChangesAsync();

            _logger.LogInformation("Jogador criado: {Nome} (Id {Id}) no time {TimeId}", jogador.Nome, jogador.Id, dto.TimeId);

            jogador.Time = time;
            return _mapper.Map<JogadorDto>(jogador);
        }

        public async Task AtualizarAsync(int id, AtualizarJogadorDto dto)
        {
            var jogador = await _jogadorRepository.GetByIdAsync(id);
            if (jogador is null)
                throw new NotFoundException($"Jogador com Id {id} não encontrado.");

            var posicao = ConverterPosicao(dto.Posicao);

            if (jogador.Numero != dto.Numero)
            {
                var numeroEmUso = await _jogadorRepository.ExisteNumeroNoTimeAsync(jogador.TimeId, dto.Numero, id);
                if (numeroEmUso)
                    throw new BusinessException($"O número {dto.Numero} já está em uso por outro jogador nesse time.");
            }

            jogador.Nome = dto.Nome;
            jogador.Numero = dto.Numero;
            jogador.Posicao = posicao;
            jogador.DataNascimento = dto.DataNascimento;

            _jogadorRepository.Update(jogador);
            await _jogadorRepository.SaveChangesAsync();

            _logger.LogInformation("Jogador {Id} atualizado com sucesso", id);
        }

        public async Task RemoverAsync(int id)
        {
            var jogador = await _jogadorRepository.GetByIdAsync(id);
            if (jogador is null)
                throw new NotFoundException($"Jogador com Id {id} não encontrado.");

            _jogadorRepository.Remove(jogador);
            await _jogadorRepository.SaveChangesAsync();

            _logger.LogInformation("Jogador {Id} removido com sucesso", id);
        }

        // Converte a string vinda do DTO (ex: "Atacante") no enum PosicaoJogador,
        // com uma mensagem de erro clara caso o valor não exista
        private static PosicaoJogador ConverterPosicao(string posicao)
        {
            if (Enum.TryParse<PosicaoJogador>(posicao, ignoreCase: true, out var resultado))
                return resultado;

            var valoresValidos = string.Join(", ", Enum.GetNames(typeof(PosicaoJogador)));
            throw new BusinessException($"Posição '{posicao}' inválida. Valores aceitos: {valoresValidos}.");
        }
    }
}