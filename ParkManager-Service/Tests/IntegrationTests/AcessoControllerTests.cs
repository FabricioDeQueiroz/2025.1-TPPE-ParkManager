using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ParkManager_Service.Helpers;
using ParkManager_Service.Models;
using ParkManager_Service.Views;

namespace ParkManager_Service.Tests.IntegrationTests
{
    public class AcessoControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        [Fact(DisplayName = "PostAcesso")]
        public async Task CadastrarAcesso()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 1 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 1 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0001",
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placa = JsonDocument.Parse(responseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";

            // Asserts
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("CAR-0001", placa);
        }

        [Fact(DisplayName = "PostAcesso - Em Evento")]
        public async Task CadastrarAcessoEvento()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 2 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 2 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            DateTime dataHoraInicio = DateTime.UtcNow.AddHours(-3).AddMinutes(-5);
            DateTime dataHoraFim = DateTime.UtcNow.AddHours(-1);

            // Cria evento para os acessos
            var novoEvento = new
            {
                nome = "Evento 1 para Teste Automatizado de Acessos",
                valorEvento = 225,
                dataHoraInicio = dataHoraInicio,
                dataHoraFim = dataHoraFim,
                idEstacionamento = idEstacionamento
            };

            using var eventoContent = new StringContent(JsonSerializer.Serialize(novoEvento), Encoding.UTF8, "application/json");

            var eventoResponse = await _client.PostAsync(new Uri("/Evento", UriKind.Relative), eventoContent).ConfigureAwait(false);
            eventoResponse.EnsureSuccessStatusCode();

            string eventoBody = await eventoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEvento = JsonDocument.Parse(eventoBody).RootElement.GetProperty("idEvento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento e evento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0002",
                idEstacionamento = idEstacionamento,
                idEvento = idEvento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placa = JsonDocument.Parse(responseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";

            // Asserts
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("CAR-0002", placa);
        }

        [Fact(DisplayName = "PostAcesso - Fora do Horário de Funcionamento")]
        public async Task CadastrarAcessoHorarioEstacionamentoFalha()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 3 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 3 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                horaAbertura = new TimeSpan(1, 0, 0),
                horaFechamento = new TimeSpan(1, 0, 0),
                tipo = TipoEstacionamento.Comum,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0003",
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string mensagem = JsonDocument.Parse(responseBody).RootElement.GetProperty("message").GetString() ?? "";

            // Asserts
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("O estacionamento está fechado no momento.", mensagem);
        }

        [Fact(DisplayName = "PostAcesso - Em Evento (fora do horário do evento)")]
        public async Task CadastrarAcessoHorarioEventoFalha()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 4 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 4 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            DateTime dataHoraInicio = DateTime.UtcNow.AddHours(-6);
            DateTime dataHoraFim = DateTime.UtcNow.AddHours(-4);

            // Cria evento para os acessos
            var novoEvento = new
            {
                nome = "Evento 2 para Teste Automatizado de Acessos",
                valorEvento = 225,
                dataHoraInicio = dataHoraInicio,
                dataHoraFim = dataHoraFim,
                idEstacionamento = idEstacionamento
            };

            using var eventoContent = new StringContent(JsonSerializer.Serialize(novoEvento), Encoding.UTF8, "application/json");

            var eventoResponse = await _client.PostAsync(new Uri("/Evento", UriKind.Relative), eventoContent).ConfigureAwait(false);
            eventoResponse.EnsureSuccessStatusCode();

            string eventoBody = await eventoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEvento = JsonDocument.Parse(eventoBody).RootElement.GetProperty("idEvento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento e evento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0004",
                idEstacionamento = idEstacionamento,
                idEvento = idEvento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string mensagem = JsonDocument.Parse(responseBody).RootElement.GetProperty("message").GetString() ?? "";

            // Asserts
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("O evento não está ativo no momento.", mensagem);
        }

        [Fact(DisplayName = "PostAcesso - Estacionamento Sem Vagas")]
        public async Task CadastrarAcessoVagasFalha()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 5 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 5 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 50, // Simula estacionamento cheio
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0005",
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string mensagem = JsonDocument.Parse(responseBody).RootElement.GetProperty("message").GetString() ?? "";

            // Asserts
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("Não há vagas disponíveis no estacionamento.", mensagem);
        }

        [Fact(DisplayName = "PutAcesso")]
        public async Task AtualizarAcesso()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 6 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 6 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0006",
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placa = JsonDocument.Parse(responseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            int idAcesso = JsonDocument.Parse(responseBody).RootElement.GetProperty("idAcesso").GetInt32();

            // Asserts
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("CAR-0006", placa);

            // Atualiza Acesso
            var acessoAtualizado = new
            {
                idAcesso = idAcesso,
                placaVeiculo = "ATT-0006"
            };

            using var updateContent = new StringContent(JsonSerializer.Serialize(acessoAtualizado), Encoding.UTF8, "application/json");

            var updateResponse = await _client.PutAsync(new Uri($"/Acesso/{idAcesso}", UriKind.Relative), updateContent).ConfigureAwait(false);

            string updateResponseBody = await updateResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placaAtualizada = JsonDocument.Parse(updateResponseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";

            // Asserts
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            Assert.Equal("ATT-0006", placaAtualizada);
        }

        [Fact(DisplayName = "DeleteAcesso")]
        public async Task DeletarAcesso()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 7 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 7 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0007",
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placa = JsonDocument.Parse(responseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            int idAcesso = JsonDocument.Parse(responseBody).RootElement.GetProperty("idAcesso").GetInt32();

            // Asserts criar
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("CAR-0007", placa);

            var deleteResponse = await _client.DeleteAsync(new Uri($"/Acesso/{idAcesso}", UriKind.Relative)).ConfigureAwait(false);

            // Asserts deletar
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync(new Uri($"/Acesso/{idAcesso}", UriKind.Relative)).ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact(DisplayName = "EncerrarAcesso - No Prazo de Tolerância")]
        public async Task EncerrarAcessoTolerancia()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 8 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 8 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0008",
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placa = JsonDocument.Parse(responseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            int idAcesso = JsonDocument.Parse(responseBody).RootElement.GetProperty("idAcesso").GetInt32();

            // Asserts criar
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("CAR-0008", placa);

            var encerrarResponse = await _client.PostAsync(new Uri($"/Acesso/finalizar/{idAcesso}", UriKind.Relative), null).ConfigureAwait(false);

            string encerrarResponseBody = await encerrarResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placaEncerrada = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            double valor = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("valorAcesso").GetDouble();
            string dataHoraSaida = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("dataHoraSaida").GetString() ?? "";

            // Asserts encerrar
            Assert.Equal(HttpStatusCode.OK, encerrarResponse.StatusCode);
            Assert.Equal("CAR-0008", placaEncerrada);
            Assert.Equal(0, valor); // tempo < 15min (tolerância) = R$ 0,00
            Assert.NotNull(dataHoraSaida);
        }

        [Fact(DisplayName = "EncerrarAcesso - PorTempo (45min)")]
        public async Task EncerrarAcessoPorTempo45Min()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 9 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 9 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0009",
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placa = JsonDocument.Parse(responseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            int idAcesso = JsonDocument.Parse(responseBody).RootElement.GetProperty("idAcesso").GetInt32();

            // Asserts criar
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("CAR-0009", placa);

            // Altera a dataHoraEntrada do acesso para simular 45 minutos atrás (-gap do UTC)
            using var db = factory.CreateDbContext();
            var acesso = await db.Acessos.FindAsync(idAcesso).ConfigureAwait(false);
            acesso!.DataHoraEntrada = DateTime.UtcNow.AddHours(-3).AddMinutes(-45).AddSeconds(-5);
            await db.SaveChangesAsync().ConfigureAwait(false);

            var encerrarResponse = await _client.PostAsync(new Uri($"/Acesso/finalizar/{idAcesso}", UriKind.Relative), null).ConfigureAwait(false);

            string encerrarResponseBody = await encerrarResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placaEncerrada = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            double valor = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("valorAcesso").GetDouble();
            string dataHoraSaida = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("dataHoraSaida").GetString() ?? "";

            // Asserts encerrar
            Assert.Equal(HttpStatusCode.OK, encerrarResponse.StatusCode);
            Assert.Equal("CAR-0009", placaEncerrada);
            Assert.Equal(7.5, valor); // 45min => 3 frações de 15min => 3 * 2.5 = R$ 7,50
            Assert.NotNull(dataHoraSaida);
        }

        [Fact(DisplayName = "EncerrarAcesso - PorTempo (1h30min)")]
        public async Task EncerrarAcessoPorTempo1H30Min()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 10 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 10 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0010",
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placa = JsonDocument.Parse(responseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            int idAcesso = JsonDocument.Parse(responseBody).RootElement.GetProperty("idAcesso").GetInt32();

            // Asserts criar
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("CAR-0010", placa);

            // Altera a dataHoraEntrada do acesso para simular 1 hora e 30 minutos atrás (-gap do UTC)
            using var db = factory.CreateDbContext();
            var acesso = await db.Acessos.FindAsync(idAcesso).ConfigureAwait(false);
            acesso!.DataHoraEntrada = DateTime.UtcNow.AddHours(-4).AddMinutes(-30).AddSeconds(-5);
            await db.SaveChangesAsync().ConfigureAwait(false);

            var encerrarResponse = await _client.PostAsync(new Uri($"/Acesso/finalizar/{idAcesso}", UriKind.Relative), null).ConfigureAwait(false);

            string encerrarResponseBody = await encerrarResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placaEncerrada = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            double valor = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("valorAcesso").GetDouble();
            string dataHoraSaida = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("dataHoraSaida").GetString() ?? "";

            // Asserts encerrar
            Assert.Equal(HttpStatusCode.OK, encerrarResponse.StatusCode);
            Assert.Equal("CAR-0010", placaEncerrada);
            Assert.Equal(13, valor); // 1h30min => 6 frações de 15min - 1 desconto hora completa) => (6 * 2.5) - 2.0 = R$ 13,00
            Assert.NotNull(dataHoraSaida);
        }

        [Fact(DisplayName = "EncerrarAcesso - Diária (24h)")]
        public async Task EncerrarAcessoDiaria()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 11 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 11 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0011",
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placa = JsonDocument.Parse(responseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            int idAcesso = JsonDocument.Parse(responseBody).RootElement.GetProperty("idAcesso").GetInt32();

            // Asserts criar
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("CAR-0011", placa);

            // Altera a dataHoraEntrada do acesso para simular 24 horas atrás (-gap do UTC)
            using var db = factory.CreateDbContext();
            var acesso = await db.Acessos.FindAsync(idAcesso).ConfigureAwait(false);
            acesso!.DataHoraEntrada = DateTime.UtcNow.AddDays(-1).AddHours(-3).AddSeconds(-5);
            await db.SaveChangesAsync().ConfigureAwait(false);

            var encerrarResponse = await _client.PostAsync(new Uri($"/Acesso/finalizar/{idAcesso}", UriKind.Relative), null).ConfigureAwait(false);

            string encerrarResponseBody = await encerrarResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placaEncerrada = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            double valor = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("valorAcesso").GetDouble();
            string dataHoraSaida = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("dataHoraSaida").GetString() ?? "";

            // Asserts encerrar
            Assert.Equal(HttpStatusCode.OK, encerrarResponse.StatusCode);
            Assert.Equal("CAR-0011", placaEncerrada);
            Assert.Equal(225, valor); // 24h => 1 diária de R$ 200,00 + R$ 25 de adicional noturno = R$ 225,55
            Assert.NotNull(dataHoraSaida);
        }

        [Fact(DisplayName = "EncerrarAcesso - Diária (25h e 30min)")]
        public async Task EncerrarAcessoDiariaTempoRestante()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 12 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 12 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0012",
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placa = JsonDocument.Parse(responseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            int idAcesso = JsonDocument.Parse(responseBody).RootElement.GetProperty("idAcesso").GetInt32();

            // Asserts criar
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("CAR-0012", placa);

            // Altera a dataHoraEntrada do acesso para simular 25 horas e 30 minutos atrás (-gap do UTC)
            using var db = factory.CreateDbContext();
            var acesso = await db.Acessos.FindAsync(idAcesso).ConfigureAwait(false);
            acesso!.DataHoraEntrada = DateTime.UtcNow.AddDays(-1).AddHours(-4).AddMinutes(-30).AddSeconds(-5);
            await db.SaveChangesAsync().ConfigureAwait(false);

            var encerrarResponse = await _client.PostAsync(new Uri($"/Acesso/finalizar/{idAcesso}", UriKind.Relative), null).ConfigureAwait(false);

            string encerrarResponseBody = await encerrarResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placaEncerrada = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            double valor = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("valorAcesso").GetDouble();
            string dataHoraSaida = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("dataHoraSaida").GetString() ?? "";

            // Asserts encerrar
            Assert.Equal(HttpStatusCode.OK, encerrarResponse.StatusCode);
            Assert.Equal("CAR-0012", placaEncerrada);
            Assert.Equal(238, valor); // 25h30min => 1 diária + 1 adicional noturno + 6 fração de 15min (1h e 30min) = 200.0 + 25.0 + ((6 *2.5) - 2.0) = R$ 238,00
            Assert.NotNull(dataHoraSaida);
        }

        [Fact(DisplayName = "EncerrarAcesso - Mensal (30 dias)")]
        public async Task EncerrarAcessoMensal()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 13 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 13 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0013",
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placa = JsonDocument.Parse(responseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            int idAcesso = JsonDocument.Parse(responseBody).RootElement.GetProperty("idAcesso").GetInt32();

            // Asserts criar
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("CAR-0013", placa);

            // Altera a dataHoraEntrada do acesso para simular 30 dias atrás (-gap de horas do UTC)
            using var db = factory.CreateDbContext();
            var acesso = await db.Acessos.FindAsync(idAcesso).ConfigureAwait(false);
            acesso!.DataHoraEntrada = DateTime.UtcNow.AddDays(-30).AddHours(-3).AddSeconds(-5);
            await db.SaveChangesAsync().ConfigureAwait(false);

            var encerrarResponse = await _client.PostAsync(new Uri($"/Acesso/finalizar/{idAcesso}", UriKind.Relative), null).ConfigureAwait(false);

            string encerrarResponseBody = await encerrarResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placaEncerrada = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            double valor = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("valorAcesso").GetDouble();
            string dataHoraSaida = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("dataHoraSaida").GetString() ?? "";

            // Asserts encerrar
            Assert.Equal(HttpStatusCode.OK, encerrarResponse.StatusCode);
            Assert.Equal("CAR-0013", placaEncerrada);
            Assert.Equal(6000, valor); // 30 dias => 1 mensal de R$ 6000,00 = R$ 6000,00
            Assert.NotNull(dataHoraSaida);
        }

        [Fact(DisplayName = "EncerrarAcesso - Mensal (33 dias 3 horas e 33 minutos)")]
        public async Task EncerrarAcessoMensalTempoRestante()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 14 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 14 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0014",
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placa = JsonDocument.Parse(responseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            int idAcesso = JsonDocument.Parse(responseBody).RootElement.GetProperty("idAcesso").GetInt32();

            // Asserts criar
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("CAR-0014", placa);

            // Altera a dataHoraEntrada do acesso para simular 33 dias, 3 horas e 33 minutos atrás (-gap de horas do UTC)
            using var db = factory.CreateDbContext();
            var acesso = await db.Acessos.FindAsync(idAcesso).ConfigureAwait(false);
            acesso!.DataHoraEntrada = DateTime.UtcNow.AddDays(-33).AddHours(-6).AddMinutes(-33).AddSeconds(-5);
            await db.SaveChangesAsync().ConfigureAwait(false);

            var encerrarResponse = await _client.PostAsync(new Uri($"/Acesso/finalizar/{idAcesso}", UriKind.Relative), null).ConfigureAwait(false);

            string encerrarResponseBody = await encerrarResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placaEncerrada = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            double valor = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("valorAcesso").GetDouble();
            string dataHoraSaida = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("dataHoraSaida").GetString() ?? "";

            // Asserts encerrar
            Assert.Equal(HttpStatusCode.OK, encerrarResponse.StatusCode);
            Assert.Equal("CAR-0014", placaEncerrada);
            Assert.Equal(6704, valor); // 33 dias 3h e 33min => 1 mensal + 3 diárias + 3 adicional noturno + 14 frações de 15min - 3 descontos hora cheia = 6000.0 + (3 * 200.0) + ((14 * 2.5) - (3 * 2.0)) = R$ 6629,00
            Assert.NotNull(dataHoraSaida);
        }

        [Fact(DisplayName = "EncerrarAcesso - Evento")]
        public async Task EncerrarAcessoEvento()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 15 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 15 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            DateTime dataHoraInicio = DateTime.UtcNow.AddHours(-4).AddMinutes(-5);
            DateTime dataHoraFim = DateTime.UtcNow.AddHours(-2);

            // Cria evento para os acessos
            var novoEvento = new
            {
                nome = "Evento 3 para Teste Automatizado de Acessos",
                valorEvento = 235,
                dataHoraInicio = dataHoraInicio,
                dataHoraFim = dataHoraFim,
                idEstacionamento = idEstacionamento
            };

            using var eventoContent = new StringContent(JsonSerializer.Serialize(novoEvento), Encoding.UTF8, "application/json");

            var eventoResponse = await _client.PostAsync(new Uri("/Evento", UriKind.Relative), eventoContent).ConfigureAwait(false);
            eventoResponse.EnsureSuccessStatusCode();

            string eventoBody = await eventoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEvento = JsonDocument.Parse(eventoBody).RootElement.GetProperty("idEvento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento e evento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0015",
                idEstacionamento = idEstacionamento,
                idEvento = idEvento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placa = JsonDocument.Parse(responseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            int idAcesso = JsonDocument.Parse(responseBody).RootElement.GetProperty("idAcesso").GetInt32();

            // Asserts
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("CAR-0015", placa);

            var encerrarResponse = await _client.PostAsync(new Uri($"/Acesso/finalizar/{idAcesso}", UriKind.Relative), null).ConfigureAwait(false);

            string encerrarResponseBody = await encerrarResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placaEncerrada = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            double valor = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("valorAcesso").GetDouble();
            string dataHoraSaida = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("dataHoraSaida").GetString() ?? "";

            // Asserts encerrar
            Assert.Equal(HttpStatusCode.OK, encerrarResponse.StatusCode);
            Assert.Equal("CAR-0015", placaEncerrada);
            Assert.Equal(235, valor); // evento => no prazo = R$ 235,00
            Assert.NotNull(dataHoraSaida);
        }

        [Fact(DisplayName = "EncerrarAcesso - Evento (Excedido)")]
        public async Task EncerrarAcessoEventoExcedido()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 16 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 16 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            DateTime dataHoraInicio = DateTime.UtcNow.AddHours(-7).AddMinutes(-5);
            DateTime dataHoraFim = DateTime.UtcNow.AddHours(-2);

            // Cria evento para os acessos
            var novoEvento = new
            {
                nome = "Evento 4 para Teste Automatizado de Acessos",
                valorEvento = 235,
                dataHoraInicio = dataHoraInicio,
                dataHoraFim = dataHoraFim,
                idEstacionamento = idEstacionamento
            };

            using var eventoContent = new StringContent(JsonSerializer.Serialize(novoEvento), Encoding.UTF8, "application/json");

            var eventoResponse = await _client.PostAsync(new Uri("/Evento", UriKind.Relative), eventoContent).ConfigureAwait(false);
            eventoResponse.EnsureSuccessStatusCode();

            string eventoBody = await eventoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEvento = JsonDocument.Parse(eventoBody).RootElement.GetProperty("idEvento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento e evento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0016",
                idEstacionamento = idEstacionamento,
                idEvento = idEvento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placa = JsonDocument.Parse(responseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            int idAcesso = JsonDocument.Parse(responseBody).RootElement.GetProperty("idAcesso").GetInt32();

            // Asserts
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("CAR-0016", placa);

            // Altera a dataHoraFim do evento para simular encerramento antes do fim do acesso
            using var db = factory.CreateDbContext();
            var evento = await db.Eventos.FindAsync(idEvento).ConfigureAwait(false);
            evento!.DataHoraFim = DateTime.UtcNow.AddHours(-5).AddMinutes(-1);
            await db.SaveChangesAsync().ConfigureAwait(false);

            var encerrarResponse = await _client.PostAsync(new Uri($"/Acesso/finalizar/{idAcesso}", UriKind.Relative), null).ConfigureAwait(false);

            string encerrarResponseBody = await encerrarResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placaEncerrada = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            double valor = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("valorAcesso").GetDouble();
            string dataHoraSaida = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("dataHoraSaida").GetString() ?? "";

            // Asserts encerrar
            Assert.Equal(HttpStatusCode.OK, encerrarResponse.StatusCode);
            Assert.Equal("CAR-0016", placaEncerrada);
            Assert.Equal(251, valor); // evento excedido => 235 + adicional (2h) => 235.0 + ((8 * 2.5) - (2 * 2.0)) = R$ 251,00
            Assert.NotNull(dataHoraSaida);
        }

        [Fact(DisplayName = "EncerrarAcesso - Saindo do Estacionamento Fechado")]
        public async Task EncerrarAcessoEstacionamentoFechadoFalha()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 17 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 17 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0017",
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placa = JsonDocument.Parse(responseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            int idAcesso = JsonDocument.Parse(responseBody).RootElement.GetProperty("idAcesso").GetInt32();

            // Asserts
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("CAR-0017", placa);

            // Altera a horaFechamento do estacionamento para simular encerramento com estacionamento fechado
            using var db = factory.CreateDbContext();
            var estacionamento = await db.Estacionamentos.FindAsync(idEstacionamento).ConfigureAwait(false);
            estacionamento!.HoraAbertura = new TimeSpan(0, 0, 0);
            estacionamento!.HoraFechamento = new TimeSpan(0, 0, 0);
            estacionamento!.Tipo = TipoEstacionamento.Comum;
            await db.SaveChangesAsync().ConfigureAwait(false);

            var encerrarResponse = await _client.PostAsync(new Uri($"/Acesso/finalizar/{idAcesso}", UriKind.Relative), null).ConfigureAwait(false);

            string encerrarResponseBody = await encerrarResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            string mensagem = JsonDocument.Parse(encerrarResponseBody).RootElement.GetProperty("message").GetString() ?? "";

            // Asserts encerrar
            Assert.Equal(HttpStatusCode.BadRequest, encerrarResponse.StatusCode);
            Assert.Equal("Aguarde a abertura do Estacionamento para finalizar seu acesso.", mensagem);
        }

        [Fact(DisplayName = "GetAcessos")]
        public async Task ListarAcessos()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 18 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 18 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0018",
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placa = JsonDocument.Parse(responseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            int idAcesso = JsonDocument.Parse(responseBody).RootElement.GetProperty("idAcesso").GetInt32();

            // Assert criar
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("CAR-0018", placa);

            var getResponse = await _client.GetAsync(new Uri($"/Acesso", UriKind.Relative)).ConfigureAwait(false);

            string getResponseBody = await getResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var acessos = JsonSerializer.Deserialize<List<AcessoGetDto>>(getResponseBody, _jsonOptions);

            // Asserts encerrar
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.NotNull(acessos);
            Assert.Contains(acessos, a => a.IdAcesso == idAcesso && a.PlacaVeiculo == placa);
        }

        [Fact(DisplayName = "GetAcesso")]
        public async Task ListarAcesso()
        {
            var (tokenUsuarioCliente, IdUsuarioCliente) = await AuthHelper.GetClienteJwtTokenAsync(_client).ConfigureAwait(false);
            var (tokenUsuarioGerente, IdUsuarioGerente) = await AuthHelper.GetGerenteJwtTokenAsync(_client).ConfigureAwait(false);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioGerente);

            // Cria estacionamento para os acessos
            var novoEstacionamento = new
            {
                nome = "Estacionamento 19 para Teste Automatizado de Acessos",
                nomeContratante = "Contratante 19 para Teste Automatizado de Acessos",
                vagasTotais = 50,
                vagasOcupadas = 0,
                faturamento = 0,
                retornoContratante = new decimal(0.55),
                valorFracao = new decimal(2.5),
                descontoHora = 2,
                valorMensal = 6000,
                valorDiaria = 200,
                adicionalNoturno = 25,
                tipo = TipoEstacionamento._24H,
                idGerente = IdUsuarioGerente
            };

            using var estacionamentoContent = new StringContent(JsonSerializer.Serialize(novoEstacionamento), Encoding.UTF8, "application/json");

            var estacionamentoResponse = await _client.PostAsync(new Uri("/Estacionamento", UriKind.Relative), estacionamentoContent).ConfigureAwait(false);
            estacionamentoResponse.EnsureSuccessStatusCode();

            string estacionamentoBody = await estacionamentoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            int idEstacionamento = JsonDocument.Parse(estacionamentoBody).RootElement.GetProperty("idEstacionamento").GetInt32();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioCliente);

            // Cria Acesso usando o estacionamento criado
            var novoAcesso = new
            {
                placaVeiculo = "CAR-0019",
                idEstacionamento = idEstacionamento
            };

            using var content = new StringContent(JsonSerializer.Serialize(novoAcesso), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(new Uri("/Acesso", UriKind.Relative), content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string placa = JsonDocument.Parse(responseBody).RootElement.GetProperty("placaVeiculo").GetString() ?? "";
            int idAcesso = JsonDocument.Parse(responseBody).RootElement.GetProperty("idAcesso").GetInt32();

            // Assert criar
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("CAR-0019", placa);

            var getResponse = await _client.GetAsync(new Uri($"/Acesso/{idAcesso}", UriKind.Relative)).ConfigureAwait(false);

            string getResponseBody = await getResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var acesso = JsonSerializer.Deserialize<AcessoGetDto>(responseBody, _jsonOptions);

            // Asserts encerrar
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.NotNull(acesso);
            Assert.Equal(idAcesso, acesso.IdAcesso);
            Assert.Equal(placa, acesso.PlacaVeiculo);
        }
    }
}
