using Microsoft.AspNetCore.Mvc;
using ParkManager_Service.Services.Interfaces;
using ParkManager_Service.Views;

namespace ParkManager_Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController(IUsuario usuarioService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUsuario([FromBody] UsuarioRegisterDto usuarioRegisterDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string? usuarioCriado = await usuarioService.RegisterAsync(usuarioRegisterDto).ConfigureAwait(false);

            if (usuarioCriado != null)
            {
                return BadRequest(usuarioCriado);
            }

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<UsuarioLoginResponseDto>> LoginUsuario([FromBody] UsuarioLoginDto usuarioLoginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await usuarioService.LoginAsync(usuarioLoginDto).ConfigureAwait(false);

            if (!usuario.Success) return BadRequest(new { message = usuario.Error });

            if (usuario.Data == null) return Unauthorized(new { message = "Erro ao realizar login." });

            return Ok(usuario);
        }
    }
}
