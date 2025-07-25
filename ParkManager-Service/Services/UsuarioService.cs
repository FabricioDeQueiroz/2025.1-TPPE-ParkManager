using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ParkManager_Service.Helpers;
using ParkManager_Service.Models;
using ParkManager_Service.Services.Interfaces;
using ParkManager_Service.Views;

namespace ParkManager_Service.Services
{
    public class UsuarioService(UserManager<Usuario> userManager, IConfiguration configuration) : IUsuario
    {
        private readonly UserManager<Usuario> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;

        public async Task<UsuarioGetDto?> GetUsuarioByIdAsync(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id).ConfigureAwait(false);

            if (usuario == null) return null;

            return new UsuarioGetDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email!,
                Tipo = usuario.Tipo
            };
        }

        public async Task<Resultado<UsuarioLoginResponseDto>> LoginAsync(UsuarioLoginDto user)
        {
            var usuario = await _userManager.FindByEmailAsync(user.Email).ConfigureAwait(false);

            if (usuario == null) return Resultado<UsuarioLoginResponseDto>.Falha("Email não encontrado.");

            bool resultado = await _userManager.CheckPasswordAsync(usuario, user.Senha).ConfigureAwait(false);

            if (!resultado) return Resultado<UsuarioLoginResponseDto>.Falha("Senha incorreta.");

            var alegacoes = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id),
                new Claim(ClaimTypes.Name, usuario.Email!),
                new Claim("Nome", usuario.Nome),
                new Claim("Email", usuario.Email!),
                new Claim(ClaimTypes.Role, usuario.Tipo.ToString())
            };

            string? chaveJwt = _configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(chaveJwt)) return Resultado<UsuarioLoginResponseDto>.Falha("A chave de autenticação está ausente ou vazia.");

            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveJwt));

            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: alegacoes,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credenciais
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Resultado<UsuarioLoginResponseDto>.Ok(
                new UsuarioLoginResponseDto
                {
                    Token = tokenString,
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email!,
                    Tipo = usuario.Tipo
                }
            );
        }

        public async Task<string?> RegisterAsync(UsuarioRegisterDto user)
        {
            var usuario = new Usuario
            {
                UserName = user.Email,
                Email = user.Email,
                Nome = user.Nome,
                Tipo = user.Tipo
            };

            var resultado = await _userManager.CreateAsync(usuario, user.Senha).ConfigureAwait(false);

            if (!resultado.Succeeded)
            {
                return resultado.Errors.FirstOrDefault()?.Description ?? "Erro desconhecido ao registrar usuário.";
            }

            return null;
        }

        public async Task<bool> DeleteUsuarioByEmailAsync(string email)
        {
            var usuario = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

            if (usuario == null) return false;

            var resultado = await _userManager.DeleteAsync(usuario).ConfigureAwait(false);

            return resultado.Succeeded;
        }
    }
}
