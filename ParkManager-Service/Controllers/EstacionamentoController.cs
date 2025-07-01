using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkManager_Service.Models;
using ParkManager_Service.Services.Interfaces;
using ParkManager_Service.Views;

namespace ParkManager_Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EstacionamentoController(IEstacionamento estacionamentoService) : ControllerBase
    {
        [Authorize(Roles = "Cliente,Gerente")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EstacionamentoGetDto>>> GetEstacionamentos()
        {
            var estacionamentos = await estacionamentoService.GetAllEstacionamentosAsync().ConfigureAwait(false);

            return Ok(estacionamentos);
        }

        [Authorize(Roles = "Cliente,Gerente")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EstacionamentoGetDto>> GetEstacionamento(int id)
        {
            var estacionamento = await estacionamentoService.GetEstacionamentoByIdAsync(id).ConfigureAwait(false);

            if (estacionamento == null)
            {
                return NotFound();
            }

            return Ok(estacionamento);
        }

        [Authorize(Roles = "Gerente")]
        [HttpPost]
        public async Task<ActionResult<EstacionamentoGetDto>> PostEstacionamento([FromBody] EstacionamentoCreateDto estacionamentoCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var novoEstacionamento = new Estacionamento
            {
                Nome = estacionamentoCreateDto.Nome,
                NomeContratante = estacionamentoCreateDto.NomeContratante,
                VagasTotais = estacionamentoCreateDto.VagasTotais,
                VagasOcupadas = estacionamentoCreateDto.VagasOcupadas,
                Faturamento = estacionamentoCreateDto.Faturamento,
                RetornoContratante = estacionamentoCreateDto.RetornoContratante,
                ValorFracao = estacionamentoCreateDto.ValorFracao,
                DescontoHora = estacionamentoCreateDto.DescontoHora,
                ValorMensal = estacionamentoCreateDto.ValorMensal,
                ValorDiaria = estacionamentoCreateDto.ValorDiaria,
                AdicionalNoturno = estacionamentoCreateDto.AdicionalNoturno,
                ValorEvento = estacionamentoCreateDto.ValorEvento,
                HoraAbertura = estacionamentoCreateDto.HoraAbertura,
                HoraFechamento = estacionamentoCreateDto.HoraFechamento,
                Tipo = estacionamentoCreateDto.Tipo
            };

            var estacionamentoCriado = await estacionamentoService.AddEstacionamentoAsync(novoEstacionamento).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetEstacionamento), new { id = estacionamentoCriado.IdEstacionamento }, estacionamentoCriado);
        }

        [Authorize(Roles = "Gerente")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<EstacionamentoGetDto>> PutEstacionamento(int id, [FromBody] EstacionamentoUpdateDto estacionamentoUpdateDto)
        {
            if (id != estacionamentoUpdateDto.IdEstacionamento)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var novosDadosEstacionamento = new Estacionamento
            {
                IdEstacionamento = id,
                Nome = estacionamentoUpdateDto.Nome,
                NomeContratante = estacionamentoUpdateDto.NomeContratante,
                VagasTotais = estacionamentoUpdateDto.VagasTotais,
                VagasOcupadas = estacionamentoUpdateDto.VagasOcupadas,
                Faturamento = estacionamentoUpdateDto.Faturamento,
                RetornoContratante = estacionamentoUpdateDto.RetornoContratante,
                ValorFracao = estacionamentoUpdateDto.ValorFracao,
                DescontoHora = estacionamentoUpdateDto.DescontoHora,
                ValorMensal = estacionamentoUpdateDto.ValorMensal,
                ValorDiaria = estacionamentoUpdateDto.ValorDiaria,
                AdicionalNoturno = estacionamentoUpdateDto.AdicionalNoturno,
                ValorEvento = estacionamentoUpdateDto.ValorEvento,
                HoraAbertura = estacionamentoUpdateDto.HoraAbertura,
                HoraFechamento = estacionamentoUpdateDto.HoraFechamento,
                Tipo = estacionamentoUpdateDto.Tipo,
                IdGerente = estacionamentoUpdateDto.IdGerente
            };

            bool success = await estacionamentoService.UpdateEstacionamentoAsync(novosDadosEstacionamento).ConfigureAwait(false);

            if (!success)
            {
                return NotFound();
            }

            var estacionamentoAtualizado = await estacionamentoService.GetEstacionamentoByIdAsync(id).ConfigureAwait(false);

            if (estacionamentoAtualizado == null)
            {
                return NotFound();
            }

            return Ok(estacionamentoAtualizado);
        }

        [Authorize(Roles = "Gerente")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEstacionamento(int id)
        {
            bool success = await estacionamentoService.DeleteEstacionamentoAsync(id).ConfigureAwait(false);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
