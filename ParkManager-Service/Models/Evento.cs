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
    [Column("valor_evento", TypeName = "numeric(10, 2)")]
    [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O valor do evento deve ser maior ou igual a zero.")]
    public decimal ValorEvento { get; set; } = 0.00m;

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

    public ICollection<Acesso> Acessos { get; } = new List<Acesso>();
}
