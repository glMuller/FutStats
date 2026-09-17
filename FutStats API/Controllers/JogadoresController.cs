using FutStatsAPI.Application.DTOs;
using FutStatsAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FutStatsAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JogadoresController : ControllerBase
    {
        private readonly IJogadorService _jogadorService;

        public JogadoresController(IJogadorService jogadorService)
        {
            _jogadorService = jogadorService;
        }

        [HttpGet("time/{timeId:int}")]
        [SwaggerOperation(Summary = "Lista os jogadores de um time específico, de forma paginada")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorTime(int timeId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var resultado = await _jogadorService.ObterPaginadoPorTimeAsync(timeId, pageNumber, pageSize);
            return Ok(resultado);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca um jogador pelo Id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var jogador = await _jogadorService.ObterPorIdAsync(id);
            return Ok(jogador);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra um novo jogador vinculado a um time")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Criar([FromBody] CriarJogadorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var jogador = await _jogadorService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = jogador.Id }, jogador);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza os dados de um jogador")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarJogadorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _jogadorService.AtualizarAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Remove um jogador")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Remover(int id)
        {
            await _jogadorService.RemoverAsync(id);
            return NoContent();
        }
    }
}