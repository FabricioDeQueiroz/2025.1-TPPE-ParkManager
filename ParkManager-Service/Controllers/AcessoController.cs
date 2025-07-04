using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkManager_Service.Services.Interfaces;
using ParkManager_Service.Views;

namespace ParkManager_Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AcessoController(IAcesso acessoService) : ControllerBase
    {
        [Authorize(Roles = "Cliente,Gerente")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcessoGetDto>>> GetAcessos()
        {
            var acessos = await acessoService.GetAllAcessosAsync().ConfigureAwait(false);

            return Ok(acessos);
        }

        [Authorize(Roles = "Cliente,Gerente")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AcessoGetDto>> GetAcesso(int id)
        {
            var acesso = await acessoService.GetAcessoByIdAsync(id).ConfigureAwait(false);

            if (acesso == null)
            {
                return NotFound();
            }

            return Ok(acesso);
        }

        [Authorize(Roles = "Cliente")]
        [HttpPost]
        public async Task<ActionResult<AcessoGetDto>> PostAcesso([FromBody] AcessoCreateDto acessoCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var acessoCriado = await acessoService.AddAcessoAsync(acessoCreateDto).ConfigureAwait(false);

            if (!acessoCriado.Success) return BadRequest(new { message = acessoCriado.Error });

            if (acessoCriado.Data == null) return BadRequest(new { message = "Erro ao realizar acesso." });

            return CreatedAtAction(nameof(GetAcesso), new { id = acessoCriado.Data.IdAcesso }, acessoCriado);
        }

        [Authorize(Roles = "Cliente")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<AcessoGetDto>> PutAcesso(int id, [FromBody] AcessoUpdateDto acessoUpdateDto)
        {
            if (id != acessoUpdateDto.IdAcesso)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool success = await acessoService.UpdateAcessoAsync(acessoUpdateDto).ConfigureAwait(false);

            if (!success)
            {
                return NotFound();
            }

            var acessoAtualizado = await acessoService.GetAcessoByIdAsync(id).ConfigureAwait(false);

            if (acessoAtualizado == null)
            {
                return NotFound();
            }

            return Ok(acessoAtualizado);
        }

        [Authorize(Roles = "Cliente")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAcesso(int id)
        {
            bool success = await acessoService.DeleteAcessoAsync(id).ConfigureAwait(false);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [Authorize(Roles = "Cliente")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AcessoGetDto>> FinalizeAcesso(int id)
        {
            var acessoFinalizado = await acessoService
                .IdentifyTypeAndValueOfAccessoAsync(id, true)
                .ConfigureAwait(false);

            if (!acessoFinalizado.Success) return BadRequest(new { message = acessoFinalizado.Error });

            if (acessoFinalizado.Data == null) return BadRequest(new { message = "Erro ao finalizar acesso." });

            return Ok(acessoFinalizado.Data);
        }
    }
}
