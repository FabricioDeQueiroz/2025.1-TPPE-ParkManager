using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ParkManager_Service.Helpers;
using ParkManager_Service.Models;
using ParkManager_Service.Views;

namespace ParkManager_Service.Tests.IntegrationTests
{
    public class EventoControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        [Fact(DisplayName = "PostEvento")]
        public async Task CadastrarEvento()
        {
            // Conteúdo
            var (tokenUsuario, IdUsuario) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuario);

            // Cria estacionamento válido
            var novoEstacionamento = new
            {
                nome = "Estacionamento para Evento Teste Automatizado",
                nomeContratante = "Contratante Evento Teste Automatizado",
                vagasTotais = 100,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 20,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuario
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            // Cria evento usando o estacionamento criado
            var novoEvento = new
            {
                nome = "Evento Teste Automatizado",
                valorEvento = 50,
                dataHoraInicio = DateTime.UtcNow.AddDays(1),
                dataHoraFim = DateTime.UtcNow.AddDays(2),
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoEvento), Encoding.UTF8, "application/json");

            // Ação
            var response = await _client.PostAsync(new Uri("/Evento", UriKind.Relative), content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string nome = JsonDocument.Parse(responseBody).RootElement.GetProperty("nome").GetString() ?? "";
            int id = JsonDocument.Parse(responseBody).RootElement.GetProperty("idEvento").GetInt32();

            // Asserts
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("Evento Teste Automatizado", nome);
            Assert.True(id > 0);

            // Limpar
            await _client.DeleteAsync(new Uri($"/Evento/{id}", UriKind.Relative)).ConfigureAwait(false);
        }

        [Fact(DisplayName = "GetEventos")]
        public async Task ListarEventos()
        {
            // Conteúdo
            var (tokenUsuario, _) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuario);

            // Ação
            var response = await _client.GetAsync(new Uri("/Evento", UriKind.Relative)).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var eventos = JsonSerializer.Deserialize<List<EventoGetDto>>(responseBody, _jsonOptions);

            // Asserts
            Assert.NotNull(eventos);
        }

        [Fact(DisplayName = "GetEventoById")]
        public async Task BuscarEventoPorId()
        {
            // Conteúdo
            var (tokenUsuario, IdUsuario) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuario);

            // Cria estacionamento válido
            var novoEstacionamento = new
            {
                nome = "Estacionamento para Evento Teste BuscarPorId",
                nomeContratante = "Contratante Evento Teste BuscarPorId",
                vagasTotais = 100,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 20,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuario
            };
            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            // Cria evento usando o estacionamento criado
            var novoEvento = new
            {
                nome = "Evento Teste BuscarPorId",
                valorEvento = 50,
                dataHoraInicio = DateTime.UtcNow.AddDays(1),
                dataHoraFim = DateTime.UtcNow.AddDays(2),
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoEvento), Encoding.UTF8, "application/json");

            // Ação
            var postResponse = await _client.PostAsync(new Uri("/Evento", UriKind.Relative), content).ConfigureAwait(false);
            postResponse.EnsureSuccessStatusCode();


            string postBody = await postResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int id = JsonDocument.Parse(postBody).RootElement.GetProperty("idEvento").GetInt32();

            // Buscar pelo id
            var response = await _client.GetAsync(new Uri($"/Evento/{id}", UriKind.Relative)).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var evento = JsonSerializer.Deserialize<EventoGetDto>(responseBody, _jsonOptions);

            // Asserts
            Assert.NotNull(evento);
            Assert.Equal(id, evento.IdEvento);

            // Limpar
            await _client.DeleteAsync(new Uri($"/Evento/{id}", UriKind.Relative)).ConfigureAwait(false);
        }

        [Fact(DisplayName = "PutEvento")]
        public async Task AtualizarEvento()
        {
            // Conteúdo
            var (tokenUsuario, IdUsuario) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuario);

            // Cria estacionamento válido
            var novoEstacionamento = new
            {
                nome = "Estacionamento para Evento Teste Atualizar",
                nomeContratante = "Contratante Evento Teste Atualizar",
                vagasTotais = 100,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 20,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuario
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            // Cria evento usando o estacionamento criado
            var novoEvento = new
            {
                nome = "Evento Teste Atualizar",
                valorEvento = 50,
                dataHoraInicio = DateTime.UtcNow.AddDays(1),
                dataHoraFim = DateTime.UtcNow.AddDays(2),
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoEvento), Encoding.UTF8, "application/json");

            // Ação
            var postResponse = await _client.PostAsync(new Uri("/Evento", UriKind.Relative), content).ConfigureAwait(false);
            postResponse.EnsureSuccessStatusCode();


            string postBody = await postResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int id = JsonDocument.Parse(postBody).RootElement.GetProperty("idEvento").GetInt32();

            // Atualiza evento
            var eventoAtualizado = new
            {
                idEvento = id,
                nome = "Evento Teste Atualizado",
                valorEvento = 50,
                dataHoraInicio = DateTime.UtcNow.AddDays(3),
                dataHoraFim = DateTime.UtcNow.AddDays(4),
                idEstacionamento = idEstacionamento
            };

            using var updateContent = new StringContent(JsonSerializer.Serialize(eventoAtualizado), Encoding.UTF8, "application/json");

            // Ação
            var putResponse = await _client.PutAsync(new Uri($"/Evento/{id}", UriKind.Relative), updateContent).ConfigureAwait(false);
            putResponse.EnsureSuccessStatusCode();


            string putBody = await putResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var evento = JsonSerializer.Deserialize<EventoGetDto>(putBody, _jsonOptions);

            // Asserts
            Assert.NotNull(evento);
            Assert.Equal("Evento Teste Atualizado", evento.Nome);

            // Limpar
            await _client.DeleteAsync(new Uri($"/Evento/{id}", UriKind.Relative)).ConfigureAwait(false);
        }

        [Fact(DisplayName = "DeleteEvento")]
        public async Task DeletarEvento()
        {
            // Conteúdo
            var (tokenUsuario, IdUsuario) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuario);

            // Cria estacionamento válido
            var novoEstacionamento = new
            {
                nome = "Estacionamento para Evento Teste Deletar",
                nomeContratante = "Contratante Evento Teste Deletar",
                vagasTotais = 100,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 20,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuario
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            // Cria evento usando o estacionamento criado
            var novoEvento = new
            {
                nome = "Evento Teste Deletar",
                valorEvento = 50,
                dataHoraInicio = DateTime.UtcNow.AddDays(1),
                dataHoraFim = DateTime.UtcNow.AddDays(2),
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoEvento), Encoding.UTF8, "application/json");

            // Ação
            var postResponse = await _client.PostAsync(new Uri("/Evento", UriKind.Relative), content).ConfigureAwait(false);
            postResponse.EnsureSuccessStatusCode();


            string postBody = await postResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int id = JsonDocument.Parse(postBody).RootElement.GetProperty("idEvento").GetInt32();

            // Ação
            var deleteResponse = await _client.DeleteAsync(new Uri($"/Evento/{id}", UriKind.Relative)).ConfigureAwait(false);

            // Asserts
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }
    }
}
