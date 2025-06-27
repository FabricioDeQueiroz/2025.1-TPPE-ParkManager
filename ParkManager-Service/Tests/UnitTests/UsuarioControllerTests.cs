using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using ParkManager_Service.Models;
using ParkManager_Service.Services;
using ParkManager_Service.Views;

namespace ParkManager_Service.Tests.UnitTests
{
    public class UsuarioControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        private readonly UsuarioService _usuarioService = new UsuarioService(
            factory.Services.GetRequiredService<UserManager<Usuario>>(),
            factory.Services.GetRequiredService<IConfiguration>()
        );

        [Fact(DisplayName = "RegisterUsuarioGerente")]
        public async Task RegisterUsuarioGerente()
        {
            var novoUsuario = new
            {
                nome = "Gerente 1 do Teste Automatizado",
                email = "gerente1automatizado@gmail.com",
                senha = "Senha12!",
                tipo = TipoUsuario.Gerente
            };

            // Conteúdo
            using var content = new StringContent(JsonSerializer.Serialize(novoUsuario), Encoding.UTF8, "application/json");

            // Ação
            var response = await _client.PostAsync(new Uri("/Usuario/register", UriKind.Relative), content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Limpar
            var delecao = await _usuarioService.DeleteUsuarioByEmailAsync(novoUsuario.email).ConfigureAwait(false);

            Assert.True(delecao, "Falha ao deletar o gerente criado para o teste.");
        }

        [Fact(DisplayName = "RegisterUsuarioCliente")]
        public async Task RegisterUsuarioCliente()
        {
            var novoUsuario = new
            {
                nome = "Cliente 1 do Teste Automatizado",
                email = "cliente1automatizado@gmail.com",
                senha = "Senha12!",
                tipo = TipoUsuario.Cliente
            };

            // Conteúdo
            using var content = new StringContent(JsonSerializer.Serialize(novoUsuario), Encoding.UTF8, "application/json");

            // Ação
            var response = await _client.PostAsync(new Uri("/Usuario/register", UriKind.Relative), content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Limpar
            var delecao = await _usuarioService.DeleteUsuarioByEmailAsync(novoUsuario.email).ConfigureAwait(false);

            Assert.True(delecao, "Falha ao deletar o cliente criado para o teste.");
        }

        [Fact(DisplayName = "LoginUsuarioGerente")]
        public async Task LoginUsuarioGerente()
        {
            // Pré-condição: Registrar um gerente para o teste
            var novoUsuario = new
            {
                nome = "Gerente 2 do Teste Automatizado",
                email = "gerente2automatizado@gmail.com",
                senha = "Senha12!",
                tipo = TipoUsuario.Gerente
            };

            // Conteúdo
            using var contentRegister = new StringContent(JsonSerializer.Serialize(novoUsuario), Encoding.UTF8, "application/json");

            // Ação
            var responseRegister = await _client.PostAsync(new Uri("/Usuario/register", UriKind.Relative), contentRegister).ConfigureAwait(false);

            // Asserts
            Assert.Equal(HttpStatusCode.OK, responseRegister.StatusCode);

            // Login
            var usuarioLogin = new UsuarioLoginDto
            {
                Email = "gerente2automatizado@gmail.com",
                Senha = "Senha12!"
            };

            // Conteúdo
            using var content = new StringContent(JsonSerializer.Serialize(usuarioLogin), Encoding.UTF8, "application/json");

            // Ação
            var response = await _client.PostAsync(new Uri("/Usuario/login", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string token = JsonDocument.Parse(responseBody).RootElement.GetProperty("token").GetString() ?? "";
            string nome = JsonDocument.Parse(responseBody).RootElement.GetProperty("nome").GetString() ?? "";

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(token);
            Assert.Equal("Gerente 2 do Teste Automatizado", nome);

            // Limpar
            var delecao = await _usuarioService.DeleteUsuarioByEmailAsync(novoUsuario.email).ConfigureAwait(false);

            Assert.True(delecao, "Falha ao deletar o gerente criado para o teste.");
        }

        [Fact(DisplayName = "LoginUsuarioCliente")]
        public async Task LoginUsuarioCliente()
        {
            // Pré-condição: Registrar um cliente para o teste
            var novoUsuario = new
            {
                nome = "Cliente 2 do Teste Automatizado",
                email = "cliente2automatizado@gmail.com",
                senha = "Senha12!",
                tipo = TipoUsuario.Cliente
            };

            // Conteúdo
            using var contentRegister = new StringContent(JsonSerializer.Serialize(novoUsuario), Encoding.UTF8, "application/json");

            // Ação
            var responseRegister = await _client.PostAsync(new Uri("/Usuario/register", UriKind.Relative), contentRegister).ConfigureAwait(false);

            // Asserts
            Assert.Equal(HttpStatusCode.OK, responseRegister.StatusCode);

            // Login
            var usuarioLogin = new UsuarioLoginDto
            {
                Email = "cliente2automatizado@gmail.com",
                Senha = "Senha12!"
            };

            // Conteúdo
            using var content = new StringContent(JsonSerializer.Serialize(usuarioLogin), Encoding.UTF8, "application/json");

            // Ação
            var response = await _client.PostAsync(new Uri("/Usuario/login", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string token = JsonDocument.Parse(responseBody).RootElement.GetProperty("token").GetString() ?? "";
            string nome = JsonDocument.Parse(responseBody).RootElement.GetProperty("nome").GetString() ?? "";

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(token);
            Assert.Equal("Cliente 2 do Teste Automatizado", nome);

            // Limpar
            var delecao = await _usuarioService.DeleteUsuarioByEmailAsync(novoUsuario.email).ConfigureAwait(false);

            Assert.True(delecao, "Falha ao deletar o cliente criado para o teste.");
        }
    }
}
