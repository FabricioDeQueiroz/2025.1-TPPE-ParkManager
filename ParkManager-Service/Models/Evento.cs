using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ParkManager_Service.Models;

[Table("EVENTO")]
public class Evento
{
    [Key]
    [Column("id_evento")]
    public int IdEvento { get; init; }

    [Required]
    [StringLength(255)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [Column("data_hora_inicio")]
    public DateTime DataHoraInicio { get; set; }

    [Required]
    [Column("data_hora_fim")]
    public DateTime DataHoraFim { get; set; }

    [Required]
    [Column("id_estacionamento")]
    public int IdEstacionamento { get; set; }

    [ForeignKey("IdEstacionamento")]
    public Estacionamento Estacionamento { get; set; } = null!;
}
