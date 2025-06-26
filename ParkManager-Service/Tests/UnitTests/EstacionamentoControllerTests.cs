using System.Net;
using System.Text;
using System.Text.Json;
using ParkManager_Service.Models;
using ParkManager_Service.Views;

namespace ParkManager_Service.Tests.UnitTests
{
    public class EstacionamentoControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        [Fact(DisplayName = "PostEstacionamento", Skip = "Ignorar")]
        public async Task CadastrarEstacionamento()
        {
            var novoEstacionamento = new
            {
                nome = "Estacionamento 1 do Teste Automatizado",
                nomeContratante = "Contratante 1 do Estacionamento do Teste Automatizado",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 20,
                valorEvento = 50,
                tipo = TipoEstacionamento._24H,
                idGerente = 1003
            };

            // Conteúdo
            using var content = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            // Ação
            var response = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string nome = JsonDocument.Parse(responseBody).RootElement.GetProperty("nome").GetString() ?? "";
            int id = JsonDocument.Parse(responseBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            // Asserts
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("Estacionamento 1 do Teste Automatizado", nome);

            // Limpar
            await _client.DeleteAsync(new Uri($"/Estacionamento/{id}", UriKind.Relative)).ConfigureAwait(false);
        }

        [Fact(DisplayName = "GetEstacionamentos", Skip = "Ignorar")]
        public async Task ListarEstacionamentos()
        {
            var novoEstacionamento = new
            {
                nome = "Estacionamento 2 do Teste Automatizado",
                nomeContratante = "Contratante 2 do Estacionamento do Teste Automatizado",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 20,
                valorEvento = 50,
                horaAbertura = new TimeSpan(7, 0, 0),
                horaFechamento = new TimeSpan(23, 59, 59),
                tipo = TipoEstacionamento.Comum,
                idGerente = 1003
            };

            // Conteúdo
            using var content = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            // Ação (Criação)
            var response = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string nome = JsonDocument.Parse(responseBody).RootElement.GetProperty("nome").GetString() ?? "";
            int id = JsonDocument.Parse(responseBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            // Asserts (Criação)
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("Estacionamento 2 do Teste Automatizado", nome);

            // Ação (Listagem)
            var responseGet = await _client.GetAsync(new Uri("/Estacionamento", UriKind.Relative)).ConfigureAwait(false);
            responseGet.EnsureSuccessStatusCode();

            string body = await responseGet.Content.ReadAsStringAsync();
            var estacionamentos = JsonSerializer.Deserialize<List<EstacionamentoGetDto>>(body, _jsonOptions);

            // Asserts (Listagem)
            Assert.Equal(HttpStatusCode.OK, responseGet.StatusCode);
            Assert.NotNull(estacionamentos);
            Assert.True(estacionamentos.Count >= 0);
            Assert.Contains(estacionamentos, e => e.Nome  == "Estacionamento 2 do Teste Automatizado");

            // Limpar
            await _client.DeleteAsync(new Uri($"/Estacionamento/{id}", UriKind.Relative)).ConfigureAwait(false);
        }

        [Fact(DisplayName = "GetEstacionamento", Skip = "Ignorar")]
        public async Task ListarEstacionamento()
        {
            var novoEstacionamento = new
            {
                nome = "Estacionamento 3 do Teste Automatizado",
                nomeContratante = "Contratante 3 do Estacionamento do Teste Automatizado",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 20,
                valorEvento = 50,
                horaAbertura = new TimeSpan(7, 0, 0),
                horaFechamento = new TimeSpan(23, 59, 59),
                tipo = TipoEstacionamento.Comum,
                idGerente = 1003
            };

            // Conteúdo
            using var content = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            // Ação (Criação)
            var response = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string nome = JsonDocument.Parse(responseBody).RootElement.GetProperty("nome").GetString() ?? "";
            int id = JsonDocument.Parse(responseBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            // Asserts (Criação)
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("Estacionamento 3 do Teste Automatizado", nome);

            // Ação (Listagem)
            var responseGet = await _client.GetAsync(new Uri($"/Estacionamento/{id}", UriKind.Relative)).ConfigureAwait(false);
            responseGet.EnsureSuccessStatusCode();

            string responseBodyGet = await responseGet.Content.ReadAsStringAsync().ConfigureAwait(false);
            string nomeGet = JsonDocument.Parse(responseBodyGet).RootElement.GetProperty("nome").GetString() ?? "";

            // Asserts (Listagem)
            Assert.Equal(HttpStatusCode.OK, responseGet.StatusCode);
            Assert.Equal("Estacionamento 3 do Teste Automatizado", nomeGet);

            // Limpar
            await _client.DeleteAsync(new Uri($"/Estacionamento/{id}", UriKind.Relative)).ConfigureAwait(false);
        }

        [Fact(DisplayName = "PutEstacionamento", Skip = "Ignorar")]
        public async Task AtualizarEstacionamento()
        {
            var novoEstacionamento = new
            {
                nome = "Estacionamento 4 do Teste Automatizado",
                nomeContratante = "Contratante 4 do Estacionamento do Teste Automatizado",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 20,
                valorEvento = 50,
                horaAbertura = new TimeSpan(5, 0, 0),
                horaFechamento = new TimeSpan(23, 0, 0),
                tipo = TipoEstacionamento.Comum,
                idGerente = 1005
            };

            // Conteúdo
            using var content = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            // Ação (Criação)
            var response = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string nome = JsonDocument.Parse(responseBody).RootElement.GetProperty("nome").GetString() ?? "";
            int id = JsonDocument.Parse(responseBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            // Asserts (Criação)
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("Estacionamento 4 do Teste Automatizado", nome);

            var atualizadoEstacionamento = new
            {
                idEstacionamento = id,
                nome = "Estacionamento 4 do Teste Automatizado Atualizado",
                nomeContratante = "Contratante 4 do Estacionamento do Teste Automatizado",
                vagasTotais = 45,
                vagasOcupadas = 5,
                faturamento = 1000,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.7),
                descontoHora = new decimal(1.8),
                valorMensal = 7000,
                valorDiaria = 250,
                adicionalNoturno = 30,
                valorEvento = 55,
                horaAbertura = new TimeSpan(5, 0, 0),
                horaFechamento = new TimeSpan(23, 59, 59),
                tipo = TipoEstacionamento.Comum,
                idGerente = 1005
            };

            // Conteúdo
            using var contentAtt = new StringContent(JsonSerializer.Serialize(atualizadoEstacionamento), Encoding.UTF8, "application/json");

            // Ação (Atualização)
            var responsePut = await _client.PutAsync(new Uri($"/Estacionamento/{id}", UriKind.Relative), contentAtt).ConfigureAwait(false);
            responsePut.EnsureSuccessStatusCode();

            string responseBodyPut = await responsePut.Content.ReadAsStringAsync().ConfigureAwait(false);
            string nomePut = JsonDocument.Parse(responseBodyPut).RootElement.GetProperty("nome").GetString() ?? "";

            // Asserts (Atualização)
            Assert.Equal(HttpStatusCode.OK, responsePut.StatusCode);
            Assert.Equal("Estacionamento 4 do Teste Automatizado Atualizado", nomePut);

            // Limpar
            await _client.DeleteAsync(new Uri($"/Estacionamento/{id}", UriKind.Relative)).ConfigureAwait(false);
        }

        [Fact(DisplayName = "DeleteEstacionamento", Skip = "Ignorar")]
        public async Task DeletarEstacionamento()
        {
            var novoEstacionamento = new
            {
                nome = "Estacionamento 5 do Teste Automatizado",
                nomeContratante = "Contratante 5 do Estacionamento do Teste Automatizado",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 20,
                valorEvento = 50,
                tipo = TipoEstacionamento._24H,
                idGerente = 1005
            };

            // Conteúdo
            using var content = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            // Ação (Criação)
            var response = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string nome = JsonDocument.Parse(responseBody).RootElement.GetProperty("nome").GetString() ?? "";
            int id = JsonDocument.Parse(responseBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            // Asserts (Criação)
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("Estacionamento 5 do Teste Automatizado", nome);

            // Ação (Deletar)
            var responseDelete = await _client.DeleteAsync(new Uri($"/Estacionamento/{id}", UriKind.Relative)).ConfigureAwait(false);
            responseDelete.EnsureSuccessStatusCode();

            // Assert (Deletar)
            Assert.Equal(HttpStatusCode.NoContent, responseDelete.StatusCode);

            // Ação (Listagem Pós-Deletar)
            var responseGet = await _client.GetAsync(new Uri($"/Estacionamento/{id}", UriKind.Relative)).ConfigureAwait(false);

            // Assert (Listagem Pós-Deletar)
            Assert.Equal(HttpStatusCode.NotFound, responseGet.StatusCode);
        }
    }
}
