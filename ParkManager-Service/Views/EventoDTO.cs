using System.ComponentModel.DataAnnotations;
using ParkManager_Service.Models;

namespace ParkManager_Service.Views
{
    public class EventoGetDto
    {
        public int IdEvento { get; init; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public EstacionamentoGetDto Estacionamento { get; set; } = null!;
    }

    public class EventoCreateDto
    {
        [Required]
        [StringLength(255)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public DateTime DataHoraInicio { get; set; }

        [Required]
        public DateTime DataHoraFim { get; set; }

        [Required]
        public int IdEstacionamento { get; init; }
    }

    public class EventoUpdateDto
    {
        [Required]
        public int IdEvento { get; init; }

        [Required]
        [StringLength(255)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public DateTime DataHoraInicio { get; set; }

        [Required]
        public DateTime DataHoraFim { get; set; }

        [Required]
        public int IdEstacionamento { get; init; }
    }
}
