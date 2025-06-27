using System.Text;
using System.Text.Json;
using ParkManager_Service.Models;
using ParkManager_Service.Views;
namespace ParkManager_Service.Helpers;

public static class AuthHelper
{
    public static async Task<(string token, string id)> GetGerenteJwtTokenAsync(HttpClient client)
    {
        //  Pré-condição: Registrar um gerente se não existir
        var gerente = new
        {
            nome = "Gerente dos Testes Automatizados",
            email = "gerentegeral@gmail.com",
            senha = "Gerente12!",
            tipo = TipoUsuario.Gerente
        };

        using var contentRegister = new StringContent(JsonSerializer.Serialize(gerente), Encoding.UTF8, "application/json");

        var responseRegister = await client.PostAsync(new Uri("/Usuario/register", UriKind.Relative), contentRegister).ConfigureAwait(false);

        // Login do gerente
        var gerenteLogin = new UsuarioLoginDto
        {
            Email = "gerentegeral@gmail.com",
            Senha = "Gerente12!"
        };

        using var contentLogin = new StringContent(JsonSerializer.Serialize(gerenteLogin), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(new Uri("/Usuario/login", UriKind.Relative), contentLogin).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        string token = JsonDocument.Parse(responseBody).RootElement.GetProperty("token").GetString() ?? "";
        string id = JsonDocument.Parse(responseBody).RootElement.GetProperty("id").GetString() ?? "";

        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Token de autenticação de gerente não foi gerado corretamente.");
        }

        if (string.IsNullOrEmpty(id))
        {
            throw new InvalidOperationException("ID de gerente não foi retornado corretamente.");
        }

        return (token, id);
    }

    public static async Task<(string token, string id)> GetClienteJwtTokenAsync(HttpClient client)
    {
        // Pré-condição: Registrar um cliente se não existir
        var cliente = new
        {
            nome = "Cliente dos Testes Automatizados",
            email = "clientegeral@gmail.com",
            senha = "Cliente12!",
            tipo = TipoUsuario.Cliente
        };

        using var contentRegister = new StringContent(JsonSerializer.Serialize(cliente), Encoding.UTF8, "application/json");

        var responseRegister = await client.PostAsync(new Uri("/Usuario/register", UriKind.Relative), contentRegister).ConfigureAwait(false);

        // Login do cliente
        var clienteLogin = new UsuarioLoginDto
        {
            Email = "clientegeral@gmail.com",
            Senha = "Cliente12!"
        };

        using var contentLogin = new StringContent(JsonSerializer.Serialize(clienteLogin), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(new Uri("/Usuario/login", UriKind.Relative), contentLogin).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        string token = JsonDocument.Parse(responseBody).RootElement.GetProperty("token").GetString() ?? "";
        string id = JsonDocument.Parse(responseBody).RootElement.GetProperty("id").GetString() ?? "";

        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Token de autenticação de cliente não foi gerado corretamente.");
        }

        if (string.IsNullOrEmpty(id))
        {
            throw new InvalidOperationException("ID de cliente não foi retornado corretamente.");
        }

        return (token, id);
    }
}
