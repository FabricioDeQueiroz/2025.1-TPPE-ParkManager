using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using ParkManager_Service.Models;
using ParkManager_Service.Services;
using ParkManager_Service.Views;

namespace ParkManager_Service.Tests.ParameterizedTests
{
    public class UsuarioParameterizedTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        private readonly UsuarioService _usuarioService = new UsuarioService(
            factory.Services.GetRequiredService<UserManager<Usuario>>(),
            factory.Services.GetRequiredService<IConfiguration>()
        );

        [Theory(DisplayName = "RegisterUsuario - Parameterized")]
        [InlineData("", "gerente0parametrizado@gmail.com", "Senha12!", TipoUsuario.Gerente)]
        [InlineData("", "cliente0parametrizado@gmail.com", "Senha12!", TipoUsuario.Cliente)]
        [InlineData("Gerente 1 do Teste Parametrizado", "gerente1parametrizado", "Senha12!", TipoUsuario.Gerente)]
        [InlineData("Cliente 1 do Teste Parametrizado", "cliente1parametrizado", "Senha12!", TipoUsuario.Cliente)]
        [InlineData("Gerente 2 do Teste Parametrizado", "gerente2parametrizado@gmail.com", "admin", TipoUsuario.Gerente)]
        [InlineData("Cliente 2 do Teste Parametrizado", "cliente2parametrizado@gmail.com", "password", TipoUsuario.Cliente)]
        [InlineData("Gerente 3 do Teste Parametrizado", "gerente3parametrizado@gmail.com", "Senha12!", 2)]
        [InlineData("Cliente 3 do Teste Parametrizado", "cliente3parametrizado@gmail.com", "Senha12!", 3)]
        public async Task RegisterUsuario(string nome, string email, string senha, TipoUsuario tipo)
        {
            var novoUsuario = new
            {
                nome,
                email,
                senha,
                tipo,
            };

            // Conteúdo
            using var content = new StringContent(JsonSerializer.Serialize(novoUsuario), Encoding.UTF8, "application/json");

            // Ação
            var response = await _client.PostAsync(new Uri("/Usuario/register", UriKind.Relative), content).ConfigureAwait(false);

            // Asserts
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory(DisplayName = "LoginUsuario - Parameterized")]
        [InlineData("Gerente 4 do Teste Parametrizado", "gerente4parametrizado@gmail.com", "", "Senha12!", "Senha12!", TipoUsuario.Gerente)]
        [InlineData("Cliente 4 do Teste Parametrizado", "cliente4parametrizado@gmail.com", "", "Senha12!", "Senha12!", TipoUsuario.Cliente)]
        [InlineData("Gerente 5 do Teste Parametrizado", "gerente5parametrizado@gmail.com", "gerente5parametrizado@gmail.com", "Senha12!", "", TipoUsuario.Gerente)]
        [InlineData("Cliente 5 do Teste Parametrizado", "cliente5parametrizado@gmail.com", "cliente5parametrizado@gmail.com", "Senha12!", "", TipoUsuario.Cliente)]
        [InlineData("Gerente 6 do Teste Parametrizado", "gerente6parametrizado@gmail.com", "gerenteparametrizado@gmail.com", "Senha12!", "Senha12!", TipoUsuario.Gerente)]
        [InlineData("Cliente 6 do Teste Parametrizado", "cliente6parametrizado@gmail.com", "clienteparametrizado@gmail.com", "Senha12!", "Senha12!", TipoUsuario.Cliente)]
        [InlineData("Gerente 7 do Teste Parametrizado", "gerente7parametrizado@gmail.com", "gerente7parametrizado@gmail.com", "Senha12!", "senha12!", TipoUsuario.Gerente)]
        [InlineData("Cliente 7 do Teste Parametrizado", "cliente7parametrizado@gmail.com", "cliente7parametrizado@gmail.com", "Senha12!", "senha12!", TipoUsuario.Cliente)]
        public async Task LoginUsuario(string nome, string email, string emailFornecido, string senha, string senhaFornecida, TipoUsuario tipo)
        {
            var novoUsuario = new
            {
                nome,
                email,
                senha,
                tipo,
            };

            // Conteúdo
            using var registerContent = new StringContent(JsonSerializer.Serialize(novoUsuario), Encoding.UTF8, "application/json");

            // Ação
            var registerResponse = await _client.PostAsync(new Uri("/Usuario/register", UriKind.Relative), registerContent).ConfigureAwait(false);

            // Asserts
            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            // Login
            var loginUsuario = new
            {
                emailFornecido,
                senhaFornecida
            };

            // Conteúdo
            using var content = new StringContent(JsonSerializer.Serialize(loginUsuario), Encoding.UTF8, "application/json");

            // Ação
            var response = await _client.PostAsync(new Uri("/Usuario/login", UriKind.Relative), content).ConfigureAwait(false);

            // Asserts
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // Limpar
            var delecao = await _usuarioService.DeleteUsuarioByEmailAsync(novoUsuario.email).ConfigureAwait(false);

            // Asserts
            Assert.True(delecao, "Falha ao deletar o cliente criado para o teste.");
        }
    }
}
