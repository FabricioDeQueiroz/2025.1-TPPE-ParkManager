using System.Text;
using System.Text.Json;
using ParkManager_Service.Models;
using ParkManager_Service.Views;
namespace ParkManager_Service.Helpers;

public static class AuthHelper
{
    public static async Task<(string token, string id)> GetGerenteJwtTokenAsync(HttpClient client)
    {
        var gerenteLogin = new UsuarioLoginDto
        {
            Email = "gerentegeral@gmail.com",
            Senha = "Gerente12!"
        };

        var (token, id) = await TentarLoginAsync(client, gerenteLogin).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(id))
            return (token, id);

        // Tenta registrar se o login falhou
        var gerente = new
        {
            nome = "Gerente dos Testes Automatizados",
            email = gerenteLogin.Email,
            senha = gerenteLogin.Senha,
            tipo = TipoUsuario.Gerente
        };

        using var contentRegister = new StringContent(JsonSerializer.Serialize(gerente), Encoding.UTF8, "application/json");
        await client.PostAsync(new Uri("/Usuario/register", UriKind.Relative), contentRegister).ConfigureAwait(false);

        return await TentarLoginAsync(client, gerenteLogin).ConfigureAwait(false);
    }

    public static async Task<(string token, string id)> GetClienteJwtTokenAsync(HttpClient client)
    {
        var clienteLogin = new UsuarioLoginDto
        {
            Email = "clientegeral@gmail.com",
            Senha = "Cliente12!"
        };

        var (token, id) = await TentarLoginAsync(client, clienteLogin).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(id))
            return (token, id);

        // Tenta registrar se o login falhou
        var cliente = new
        {
            nome = "Cliente dos Testes Automatizados",
            email = clienteLogin.Email,
            senha = clienteLogin.Senha,
            tipo = TipoUsuario.Cliente
        };

        using var contentRegister = new StringContent(JsonSerializer.Serialize(cliente), Encoding.UTF8, "application/json");
        await client.PostAsync(new Uri("/Usuario/register", UriKind.Relative), contentRegister).ConfigureAwait(false);

        return await TentarLoginAsync(client, clienteLogin).ConfigureAwait(false);
    }

    private static async Task<(string token, string id)> TentarLoginAsync(HttpClient client, UsuarioLoginDto loginDto)
    {
        using var contentLogin = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(new Uri("/Usuario/login", UriKind.Relative), contentLogin).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            return ("", "");

        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var json = JsonDocument.Parse(responseBody).RootElement;

        string token = json.GetProperty("token").GetString() ?? "";
        string id = json.GetProperty("id").GetString() ?? "";

        return (token, id);
    }
}
