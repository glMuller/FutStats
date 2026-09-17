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
    public class TimeServiceTests
    {
        private readonly Mock<ITimeRepository> _repoMock = new();
        private readonly IMapper _mapper;
        private readonly TimeService _service;

        public TimeServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
            _service = new TimeService(_repoMock.Object, _mapper, Mock.Of<ILogger<TimeService>>());
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoNomeJaExiste()
        {
            // Arrange
            _repoMock.Setup(r => r.ExisteComNomeAsync("Flamengo")).ReturnsAsync(true);
            var dto = new CriarTimeDto { Nome = "Flamengo", Cidade = "Rio de Janeiro", Estado = "RJ", AnoFundacao = 1895 };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _service.CriarAsync(dto));
        }

        [Fact]
        public async Task CriarAsync_DeveCriarTime_QuandoNomeNaoExiste()
        {
            // Arrange
            _repoMock.Setup(r => r.ExisteComNomeAsync(It.IsAny<string>())).ReturnsAsync(false);
            var dto = new CriarTimeDto { Nome = "Palmeiras", Cidade = "São Paulo", Estado = "SP", AnoFundacao = 1914 };

            // Act
            var resultado = await _service.CriarAsync(dto);

            // Assert
            Assert.Equal("Palmeiras", resultado.Nome);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Time>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveLancarNotFound_QuandoTimeNaoExiste()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdComJogadoresAsync(It.IsAny<int>())).ReturnsAsync((Time?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.ObterPorIdAsync(99));
        }
    }
}