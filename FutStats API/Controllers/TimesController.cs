using FutStatsAPI.Application.DTOs;
using FutStatsAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FutStatsAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimesController : ControllerBase
    {
        private readonly ITimeService _timeService;

        public TimesController(ITimeService timeService)
        {
            _timeService = timeService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista os times cadastrados de forma paginada")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTodos([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var resultado = await _timeService.ObterPaginadoAsync(pageNumber, pageSize);
            return Ok(resultado);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca um time pelo Id, incluindo a lista de jogadores")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var time = await _timeService.ObterPorIdAsync(id);
            return Ok(time);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra um novo time")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Criar([FromBody] CriarTimeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var time = await _timeService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = time.Id }, time);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza os dados de um time existente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarTimeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _timeService.AtualizarAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Remove um time")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Remover(int id)
        {
            await _timeService.RemoverAsync(id);
            return NoContent();
        }
    }
}