using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ParkManager_Service.Migrations
{
    /// <inheritdoc />
    public partial class SchemaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USUARIO",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    senha = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO", x => x.id_usuario);
                });

            migrationBuilder.CreateTable(
                name: "ESTACIONAMENTO",
                columns: table => new
                {
                    id_estacionamento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    nome_contratante = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    vagas_totais = table.Column<int>(type: "integer", nullable: false),
                    vagas_ocupadas = table.Column<int>(type: "integer", nullable: false),
                    faturamento = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    retorno_contratante = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    valor_fracao = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    desconto_hora = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    valor_mensal = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    valor_diaria = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    adicional_noturno = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    valor_evento = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    hora_abertura = table.Column<TimeSpan>(type: "interval", nullable: true),
                    hora_fechamento = table.Column<TimeSpan>(type: "interval", nullable: true),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    id_gerente = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESTACIONAMENTO", x => x.id_estacionamento);
                    table.ForeignKey(
                        name: "FK_ESTACIONAMENTO_USUARIO_id_gerente",
                        column: x => x.id_gerente,
                        principalTable: "USUARIO",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ACESSO",
                columns: table => new
                {
                    id_acesso = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    placa_veiculo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    valor_acesso = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    data_hora_entrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_hora_saida = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    nome_evento = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    id_cliente = table.Column<int>(type: "integer", nullable: false),
                    id_estacionamento = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ACESSO", x => x.id_acesso);
                    table.ForeignKey(
                        name: "FK_ACESSO_ESTACIONAMENTO_id_estacionamento",
                        column: x => x.id_estacionamento,
                        principalTable: "ESTACIONAMENTO",
                        principalColumn: "id_estacionamento",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ACESSO_USUARIO_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "USUARIO",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EVENTO",
                columns: table => new
                {
                    id_evento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    data_hora_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_hora_fim = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    id_estacionamento = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVENTO", x => x.id_evento);
                    table.ForeignKey(
                        name: "FK_EVENTO_ESTACIONAMENTO_id_estacionamento",
                        column: x => x.id_estacionamento,
                        principalTable: "ESTACIONAMENTO",
                        principalColumn: "id_estacionamento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ACESSO_id_cliente",
                table: "ACESSO",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_ACESSO_id_estacionamento",
                table: "ACESSO",
                column: "id_estacionamento");

            migrationBuilder.CreateIndex(
                name: "IX_ESTACIONAMENTO_id_gerente",
                table: "ESTACIONAMENTO",
                column: "id_gerente");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_id_estacionamento",
                table: "EVENTO",
                column: "id_estacionamento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ACESSO");

            migrationBuilder.DropTable(
                name: "EVENTO");

            migrationBuilder.DropTable(
                name: "ESTACIONAMENTO");

            migrationBuilder.DropTable(
                name: "USUARIO");
        }
    }
}
