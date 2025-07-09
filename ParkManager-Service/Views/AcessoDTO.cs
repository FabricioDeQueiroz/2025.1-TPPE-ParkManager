using System.ComponentModel.DataAnnotations;
using ParkManager_Service.Models;

namespace ParkManager_Service.Views
{
    public class AcessoGetDto
    {
        public int IdAcesso { get; init; }
        public string PlacaVeiculo { get; set; } = string.Empty;
        public decimal? ValorAcesso { get; set; }
        public DateTime DataHoraEntrada { get; set; }
        public DateTime? DataHoraSaida { get; set; }
        public TipoAcesso? Tipo { get; set; }
        public UsuarioGetDto Cliente { get; set; } = null!;
        public EstacionamentoGetDto Estacionamento { get; set; } = null!;
        public EventoForAcessoGetDto? Evento { get; set; } = null!;
    }

    public class AcessoCreateDto
    {
        [Required]
        [StringLength(10)]
        public string PlacaVeiculo { get; set; } = string.Empty;

        [Required]
        public int IdEstacionamento { get; init; }

        public int? IdEvento { get; init; }
    }

    public class AcessoUpdateDto
    {
        [Required]
        public int IdAcesso { get; init; }

        [Required]
        [StringLength(10)]
        public string PlacaVeiculo { get; set; } = string.Empty;
    }
}
