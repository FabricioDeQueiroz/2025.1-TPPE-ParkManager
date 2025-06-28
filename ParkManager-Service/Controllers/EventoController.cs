using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkManager_Service.Models;
using ParkManager_Service.Services.Interfaces;
using ParkManager_Service.Views;

namespace ParkManager_Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventoController(IEvento eventoService) : ControllerBase
    {
        [Authorize(Roles = "Cliente,Gerente")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoGetDto>>> GetEventos()
        {
            var eventos = await eventoService.GetAllEventosAsync().ConfigureAwait(false);

            return Ok(eventos);
        }

        [Authorize(Roles = "Cliente,Gerente")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EventoGetDto>> GetEvento(int id)
        {
            var evento = await eventoService.GetEventoByIdAsync(id).ConfigureAwait(false);

            if (evento == null)
            {
                return NotFound();
            }

            return Ok(evento);
        }

        [Authorize(Roles = "Gerente")]
        [HttpPost]
        public async Task<ActionResult<EventoGetDto>> PostEvento([FromBody] EventoCreateDto eventoCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var eventoCriado = await eventoService.AddEventoAsync(eventoCreateDto).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetEvento), new { id = eventoCriado.IdEvento }, eventoCriado);
        }

        [Authorize(Roles = "Gerente")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<EventoGetDto>> PutEvento(int id, [FromBody] EventoUpdateDto eventoUpdateDto)
        {
            if (id != eventoUpdateDto.IdEvento)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool success = await eventoService.UpdateEventoAsync(eventoUpdateDto).ConfigureAwait(false);

            if (!success)
            {
                return NotFound();
            }

            var eventoAtualizado = await eventoService.GetEventoByIdAsync(id).ConfigureAwait(false);

            if (eventoAtualizado == null)
            {
                return NotFound();
            }

            return Ok(eventoAtualizado);
        }

        [Authorize(Roles = "Gerente")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            bool success = await eventoService.DeleteEventoAsync(id).ConfigureAwait(false);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
