using System.Globalization;
using Bogus;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkManager_Service.Migrations
{
    /// <inheritdoc />
    public partial class FakesMigration : Migration
    {
        private static readonly string[] nomesEventos = new[] { "Nenhum", "Show de Rock", "Feira Tech", "Festa Anual" };
        private static readonly string[] columnsEvento = new[] { "id_evento", "nome", "data_hora_inicio", "data_hora_fim", "id_estacionamento" };
        private static readonly string[] columnsUsuario = new[] { "Id", "nome", "tipo", "UserName", "Email", "EmailConfirmed", "PasswordHash", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount" };
        private static readonly string[] columnsAcesso = new[] {
            "id_acesso", "placa_veiculo", "valor_acesso", "data_hora_entrada",
            "data_hora_saida", "nome_evento", "tipo", "id_cliente", "id_estacionamento"
        };
        private static readonly string[] columnsEstacionamento = new[] {
            "id_estacionamento", "nome", "nome_contratante", "vagas_totais", "vagas_ocupadas",
            "faturamento", "retorno_contratante", "valor_fracao", "desconto_hora",
            "valor_mensal", "valor_diaria", "adicional_noturno", "valor_evento",
            "hora_abertura", "hora_fechamento", "tipo", "id_gerente"
        };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // -------------------------------------------------------------
            // INSERTS DE DADOS FAKES (POPULA)
            // -------------------------------------------------------------

            var faker = new Faker("pt_BR");

            // 1. Inserir USUARIO (100 usuários: 20 gerentes, 80 clientes)
            var usersToInsert = new List<object[]>();
            var gerenteIds = new List<string>();
            var clienteIds = new List<string>();

            // Gerentes
            for (int i = 1; i <= 20; i++)
            {
                var userId = 1000 + i;
                var email = faker.Internet.Email();
                gerenteIds.Add(userId.ToString(CultureInfo.InvariantCulture));
                usersToInsert.Add(new object[]
                {
                    userId.ToString(CultureInfo.InvariantCulture),
                    faker.Name.FullName(),
                    0,
                    email,
                    email,
                    true,
                    faker.Internet.Password(),
                    true,
                    true,
                    true,
                    0
                });
            }

            // Clientes
            for (int i = 1; i <= 80; i++)
            {
                var userId = 1000 + 20 + i;
                var email = faker.Internet.Email();
                clienteIds.Add(userId.ToString(CultureInfo.InvariantCulture));
                usersToInsert.Add(new object[]
                {
                    userId.ToString(CultureInfo.InvariantCulture),
                    faker.Name.FullName(),
                    1,
                    email,
                    email,
                    true,
                    faker.Internet.Password(),
                    true,
                    true,
                    true,
                    0
                });
            }

            for (int i = 1; i <= 100; i++)
            {
                migrationBuilder.InsertData(
                    table: "AspNetUsers",
                    columns: columnsUsuario,
                    values: usersToInsert.ToArray()[i - 1]
                );
            }

            // 2. Inserir ESTACIONAMENTO (100 estacionamentos)
            var estacionamentosToInsert = new List<object[]>();
            var estacionamentoIds = new List<int>();
            var random = new Random();

            for (int i = 1; i <= 100; i++)
            {
                var estId = 2000 + i;
                estacionamentoIds.Add(estId);
                var vagasTotais = random.Next(50, 500);
                var vagasOcupadas = random.Next(0, vagasTotais);
                var gerenteId = faker.PickRandom(gerenteIds);

                var estacionamentoTipo = random.Next(0, 2);

                object horaAbertura = null;
                object horaFechamento = null;

                if (estacionamentoTipo == 0)
                {
                    // Estacionamento Comum
                    horaAbertura = TimeSpan.FromHours(random.Next(6, 9));
                    horaFechamento = TimeSpan.FromHours(random.Next(18, 23));
                }
                else
                {
                    // Estacionamento 24H
                    horaAbertura = null;
                    horaFechamento = null;
                }

                estacionamentosToInsert.Add(new object[]
                {
                    estId,
                    faker.Company.CompanyName() + " Estacionamento " + faker.Address.StreetName(),
                    faker.Company.CompanyName() + " Contratante",
                    vagasTotais,
                    vagasOcupadas,
                    faker.Finance.Amount(1000m, 100000m),
                    faker.Finance.Amount(500m, 50000m),
                    faker.Finance.Amount(5m, 20m),
                    faker.Finance.Amount(1m, 5m),
                    faker.Finance.Amount(100m, 500m),
                    faker.Finance.Amount(20m, 80m),
                    faker.Finance.Amount(1m, 10m),
                    faker.Finance.Amount(30m, 150m),
                    horaAbertura,
                    horaFechamento,
                    estacionamentoTipo,
                    gerenteId
                });
            }

            for (int i = 1; i <= 100; i++)
            {
                migrationBuilder.InsertData(
                    table: "ESTACIONAMENTO",
                    columns: columnsEstacionamento,
                    values: estacionamentosToInsert.ToArray()[i - 1]
                );
            }

            // 3. Inserir EVENTO (100 eventos)
            var eventosToInsert = new List<object[]>();

            for (int i = 1; i <= 100; i++)
            {
                var eventoId = 3000 + i;
                var dataInicio = faker.Date.Soon(30);
                var dataFim = dataInicio.AddHours(random.Next(2, 8));
                var estacionamentoParaEvento = faker.PickRandom(estacionamentoIds);

                eventosToInsert.Add(new object[]
                {
                    eventoId,
                    faker.Lorem.Sentence(3, 5) + " Evento",
                    dataInicio.ToUniversalTime(),
                    dataFim.ToUniversalTime(),
                    estacionamentoParaEvento
                });
            }

            for (int i = 1; i <= 100; i++)
            {
                migrationBuilder.InsertData(
                    table: "EVENTO",
                    columns: columnsEvento,
                    values: eventosToInsert.ToArray()[i - 1]
                );
            }

            // 4. Inserir ACESSO (100 acessos)
            var acessosToInsert = new List<object[]>();

            for (int i = 1; i <= 100; i++)
            {
                var acessoId = 4000 + i;
                var dataEntrada = faker.Date.Past(30);
                var dataSaida = dataEntrada.AddMinutes(random.Next(30, 86400));

                var clienteDoAcesso = faker.PickRandom(clienteIds);
                var estacionamentoDoAcesso = faker.PickRandom(estacionamentoIds);
                var nomeEventoAcesso = faker.PickRandom(nomesEventos);

                acessosToInsert.Add(new object[]
                {
                    acessoId,
                    faker.Random.AlphaNumeric(3).ToUpper(CultureInfo.CurrentCulture) + "-" + faker.Random.Number(1000, 9999),
                    faker.Finance.Amount(5m, 100m),
                    dataEntrada.ToUniversalTime(),
                    dataSaida.ToUniversalTime(),
                    nomeEventoAcesso,
                    random.Next(0, 4),
                    clienteDoAcesso,
                    estacionamentoDoAcesso
                });
            }

            for (int i = 1; i <= 100; i++)
            {
                migrationBuilder.InsertData(
                    table: "ACESSO",
                    columns: columnsAcesso,
                    values: acessosToInsert.ToArray()[i - 1]
                );
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // -------------------------------------------------------------
            // DELETES DE DADOS FAKES (APAGAR) - Ordem inversa dos inserts
            // -------------------------------------------------------------

            // 1. Deletar ACESSO (IDs 4001 a 4100)
            for (int i = 1; i <= 100; i++)
            {
                migrationBuilder.DeleteData(
                    table: "ACESSO",
                    keyColumn: "id_acesso",
                    keyValue: 4000 + i
                );
            }

            // 2. Deletar EVENTO (IDs 3001 a 3100)
            for (int i = 1; i <= 100; i++)
            {
                migrationBuilder.DeleteData(
                    table: "EVENTO",
                    keyColumn: "id_evento",
                    keyValue: 3000 + i
                );
            }

            // 3. Deletar ESTACIONAMENTO (IDs 2001 a 2100)
            for (int i = 1; i <= 100; i++)
            {
                migrationBuilder.DeleteData(
                    table: "ESTACIONAMENTO",
                    keyColumn: "id_estacionamento",
                    keyValue: 2000 + i
                );
            }

            // 4. Deletar USUARIO (IDs 1001 a 1100)
            for (int i = 1; i <= 100; i++)
            {
                migrationBuilder.DeleteData(
                    table: "AspNetUsers",
                    keyColumn: "Id",
                    keyValue: (1000 + i).ToString(CultureInfo.InvariantCulture)
                );
            }
        }
    }
}
