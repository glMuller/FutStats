using AutoMapper;
using FutStatsAPI.Application.DTOs;
using FutStatsAPI.Application.Exceptions;
using FutStatsAPI.Application.Mappings;
using FutStatsAPI.Application.Services;
using FutStatsAPI.Domain.Entities;
using FutStatsAPI.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FutStatsAPI.Tests.Unit.Services
{
    public class JogadorServiceTests
    {
        private readonly Mock<IJogadorRepository> _jogadorRepoMock = new();
        private readonly Mock<ITimeRepository> _timeRepoMock = new();
        private readonly IMapper _mapper;
        private readonly JogadorService _service;

        public JogadorServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
            _service = new JogadorService(_jogadorRepoMock.Object, _timeRepoMock.Object, _mapper, Mock.Of<ILogger<JogadorService>>());
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoPosicaoInvalida()
        {
            // Arrange
            _timeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Time { Id = 1, Nome = "Santos" });
            var dto = new CriarJogadorDto { Nome = "Jogador X", Numero = 10, Posicao = "PosicaoQueNaoExiste", TimeId = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _service.CriarAsync(dto));
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoNumeroJaEstaEmUso()
        {
            // Arrange
            _timeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Time { Id = 1, Nome = "Santos" });
            _jogadorRepoMock.Setup(r => r.ExisteNumeroNoTimeAsync(1, 10, null)).ReturnsAsync(true);
            var dto = new CriarJogadorDto { Nome = "Jogador X", Numero = 10, Posicao = "Atacante", TimeId = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _service.CriarAsync(dto));
        }

        [Fact]
        public async Task CriarAsync_DeveLancarNotFound_QuandoTimeNaoExiste()
        {
            // Arrange
            _timeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Time?)null);
            var dto = new CriarJogadorDto { Nome = "Jogador X", Numero = 10, Posicao = "Atacante", TimeId = 99 };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CriarAsync(dto));
        }
    }
}