using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ParkManager_Service.Models;

public enum TipoAcesso
{
    PorTempo,
    Diaria,
    Mensal,
    Evento
}

[Table("ACESSO")]
public class Acesso
{
    [Key]
    [Column("id_acesso")]
    public int IdAcesso { get; init; }

    [Required]
    [StringLength(10)]
    [Column("placa_veiculo")]
    public string PlacaVeiculo { get; set; } = string.Empty;

    [Column("valor_acesso", TypeName = "numeric(10, 2)")]
    public decimal? ValorAcesso { get; set; }

    [Required]
    [Column("data_hora_entrada")]
    public DateTime DataHoraEntrada { get; set; }

    [Column("data_hora_saida")]
    public DateTime? DataHoraSaida { get; set; }

    [Required]
    [Column("tipo")]
    [EnumDataType(typeof(TipoAcesso))]
    public TipoAcesso Tipo { get; set; }

    [Required]
    [Column("id_cliente")]
    public string IdCliente { get; set; } = string.Empty;

    [Required]
    [Column("id_estacionamento")]
    public int IdEstacionamento { get; set; }

    [Column("id_evento")]
    public int? IdEvento { get; set; }

    [ForeignKey("IdCliente")]
    public Usuario Cliente { get; set; } = null!;

    [ForeignKey("IdEstacionamento")]
    public Estacionamento Estacionamento { get; set; } = null!;

    [ForeignKey("IdEvento")]
    public Evento? Evento { get; set; }
}
