using System.Net;
using System.Net.Http.Json;
using FutStatsAPI.Application.DTOs;
using Xunit;

namespace FutStatsAPI.Tests.Integration
{
    public class TimesControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public TimesControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_DeveCriarTime_ERetornar201()
        {
            // Arrange
            var dto = new CriarTimeDto { Nome = "Corinthians", Cidade = "São Paulo", Estado = "SP", AnoFundacao = 1910 };

            // Act
            var response = await _client.PostAsJsonAsync("/api/times", dto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<TimeDto>();
            Assert.NotNull(body);
            Assert.Equal("Corinthians", body!.Nome);
        }

        [Fact]
        public async Task Post_DeveRetornar400_QuandoNomeDuplicado()
        {
            // Arrange: cria o mesmo time duas vezes
            var dto = new CriarTimeDto { Nome = "São Paulo FC", Cidade = "São Paulo", Estado = "SP", AnoFundacao = 1930 };
            await _client.PostAsJsonAsync("/api/times", dto);

            // Act
            var response = await _client.PostAsJsonAsync("/api/times", dto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_DeveRetornarListaPaginada()
        {
            // Act
            var response = await _client.GetAsync("/api/times?pageNumber=1&pageSize=5");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetPorId_DeveRetornar404_QuandoNaoExiste()
        {
            // Act
            var response = await _client.GetAsync("/api/times/999999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}