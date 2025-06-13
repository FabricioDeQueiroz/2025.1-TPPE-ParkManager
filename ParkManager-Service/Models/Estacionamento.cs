using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ParkManager_Service.Models;

public enum TipoEstacionamento
{
    Comum,
    _24H
}

[Table("ESTACIONAMENTO")]
public class Estacionamento
{
    [Key]
    [Column("id_estacionamento")]
    public int IdEstacionamento { get; init; }

    [Required]
    [StringLength(255)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    [Column("nome_contratante")]
    public string NomeContratante { get; set; } = string.Empty;

    [Required]
    [Column("vagas_totais")]
    [Range(1, int.MaxValue, ErrorMessage = "O número de vagas totais deve ser maior ou igual a um.")]
    public int VagasTotais { get; set; } = 1;

    [Required]
    [Column("vagas_ocupadas")]
    [Range(0, int.MaxValue, ErrorMessage = "O número de vagas ocupadas deve ser maior ou igual a zero.")]
    public int VagasOcupadas { get; set; } = 0;

    [Required]
    [Column("faturamento", TypeName = "numeric(10, 2)")]
    public decimal Faturamento { get; set; } = 0.00m;

    [Required]
    [Column("retorno_contratante", TypeName = "numeric(10, 2)")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor do retorno ao contratante deve ser maior que zero.")]
    public decimal RetornoContratante { get; set; } = 0.01m;

    [Required]
    [Column("valor_fracao", TypeName = "numeric(10, 2)")]
    [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O valor da fração deve ser maior ou igual a zero.")]
    public decimal ValorFracao { get; set; } = 0.00m;

    [Required]
    [Column("desconto_hora", TypeName = "numeric(10, 2)")]
    [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O desconto por hora deve ser maior ou igual a zero.")]
    public decimal DescontoHora { get; set; } = 0.00m;

    [Required]
    [Column("valor_mensal", TypeName = "numeric(10, 2)")]
    [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O valor mensal deve ser maior ou igual a zero.")]
    public decimal ValorMensal { get; set; } = 0.00m;

    [Required]
    [Column("valor_diaria", TypeName = "numeric(10, 2)")]
    [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O valor da diária deve ser maior ou igual a zero.")]
    public decimal ValorDiaria { get; set; } = 0.00m;

    [Required]
    [Column("adicional_noturno", TypeName = "numeric(10, 2)")]
    [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O adicional noturno deve ser maior ou igual a zero.")]
    public decimal AdicionalNoturno { get; set; } = 0.00m;

    [Required]
    [Column("valor_evento", TypeName = "numeric(10, 2)")]
    [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O valor do evento deve ser maior ou igual a zero.")]
    public decimal ValorEvento { get; set; } = 0.00m;

    [Column("hora_abertura")]
    public TimeSpan? HoraAbertura { get; set; } = null;

    [Column("hora_fechamento")]
    public TimeSpan? HoraFechamento { get; set; } = null;

    [Required]
    [Column("tipo")]
    [EnumDataType(typeof(TipoEstacionamento))]
    public TipoEstacionamento Tipo { get; set; }

    [Required]
    [Column("id_gerente")]
    public int IdGerente { get; set; }

    [ForeignKey("IdGerente")]
    public Usuario Gerente { get; set; } = null!;

    public ICollection<Evento> Eventos { get; } = new List<Evento>();
    public ICollection<Acesso> Acessos { get; } = new List<Acesso>();
}
