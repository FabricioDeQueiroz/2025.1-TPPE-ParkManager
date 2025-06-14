using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ParkManager_Service.Controllers;
using ParkManager_Service.Models;
using ParkManager_Service.Services.Interfaces;
using ParkManager_Service.Views;

namespace ParkManager_Service.Tests.UnitTests
{
    public class EstacionamentoControllerTests
    {
        private readonly Mock<IEstacionamento> _mockEstacionamentoService;
        private readonly EstacionamentoController _controller;

        public EstacionamentoControllerTests()
        {
            _mockEstacionamentoService = new Mock<IEstacionamento>();
            _controller = new EstacionamentoController(_mockEstacionamentoService.Object);
        }

        [Fact(DisplayName = "GetEstacionamentos - Com Dados")]
        public async Task GetEstacionamentosRetornaOkComEstacionamentos()
        {
            // Arrange
            var estacionamentosDto = new List<EstacionamentoGetDto>
            {
                new EstacionamentoGetDto
                {
                    IdEstacionamento = 2001,
                    Nome = "Estacionamento A",
                    NomeContratante = "Contratante A",
                    VagasTotais = 5,
                    VagasOcupadas = 1,
                    Faturamento = 0,
                    RetornoContratante = new decimal(0.5),
                    ValorFracao = 5,
                    DescontoHora = 2,
                    ValorMensal = 5,
                    ValorDiaria = 5,
                    AdicionalNoturno = 5,
                    ValorEvento = 5,
                    HoraAbertura = TimeSpan.Parse("08:00:00", null),
                    HoraFechamento = TimeSpan.Parse("23:00:00", null),
                    Tipo = TipoEstacionamento.Comum,
                    IdGerente = 1002
                },
                new EstacionamentoGetDto
                {
                    IdEstacionamento = 2002,
                    Nome = "Estacionamento B",
                    NomeContratante = "Contratante B",
                    VagasTotais = 5,
                    VagasOcupadas = 1,
                    Faturamento = 0,
                    RetornoContratante = new decimal(0.5),
                    ValorFracao = 5,
                    DescontoHora = 2,
                    ValorMensal = 5,
                    ValorDiaria = 5,
                    AdicionalNoturno = 5,
                    ValorEvento = 5,
                    HoraAbertura = null,
                    HoraFechamento = null,
                    Tipo = TipoEstacionamento._24H,
                    IdGerente = 1003
                }
            };

            _mockEstacionamentoService.Setup(s => s.GetAllEstacionamentosAsync())
                .ReturnsAsync(estacionamentosDto);

            // Act
            var result = await _controller.GetEstacionamentos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<EstacionamentoGetDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
            Assert.Equal("Estacionamento A", returnValue[0].Nome);
        }

        [Fact(DisplayName = "GetEstacionamento - Por ID")]
        public async Task GetEstacionamentoPorId()
        {
            // Arrange
            var estacionamentoDto = new EstacionamentoGetDto
            {
                IdEstacionamento = 2003,
                Nome = "Estacionamento Teste",
                NomeContratante = "Contratante Teste",
                VagasTotais = 5,
                VagasOcupadas = 1,
                Faturamento = 0,
                RetornoContratante = new decimal(0.5),
                ValorFracao = 5,
                DescontoHora = 2,
                ValorMensal = 5,
                ValorDiaria = 5,
                AdicionalNoturno = 5,
                ValorEvento = 5,
                HoraAbertura = null,
                HoraFechamento = null,
                Tipo = TipoEstacionamento._24H,
                IdGerente = 1003
            };

            _mockEstacionamentoService.Setup(s => s.GetEstacionamentoByIdAsync(2003))
                .ReturnsAsync(estacionamentoDto);

            // Act
            var result = await _controller.GetEstacionamento(2003);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<EstacionamentoGetDto>(okResult.Value);
            Assert.Equal(2003, returnValue.IdEstacionamento);
            Assert.Equal("Estacionamento Teste", returnValue.Nome);
        }
    }
}
