using System.ComponentModel.DataAnnotations;
using ParkManager_Service.Models;

namespace ParkManager_Service.Views
{
    public class EstacionamentoGetDto
    {
        public int IdEstacionamento { get; init; }
        public string Nome { get; set; } = string.Empty;
        public string NomeContratante { get; set; } = string.Empty;
        public int VagasTotais { get; set; }
        public int VagasOcupadas { get; set; }
        public decimal Faturamento { get; set; }
        public decimal RetornoContratante { get; set; }
        public decimal ValorFracao { get; set; }
        public decimal DescontoHora { get; set; }
        public decimal ValorMensal { get; set; }
        public decimal ValorDiaria { get; set; }
        public decimal AdicionalNoturno { get; set; }
        public TimeSpan? HoraAbertura { get; set; }
        public TimeSpan? HoraFechamento { get; set; }
        public TipoEstacionamento Tipo { get; set; }
        public string IdGerente { get; set; } = string.Empty;
    }

    public class EstacionamentoCreateDto
    {
        [Required]
        [StringLength(255)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string NomeContratante { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "O número de vagas totais deve ser maior ou igual a um.")]
        public int VagasTotais { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "O número de vagas ocupadas deve ser maior ou igual a zero.")]
        public int VagasOcupadas { get; set; }

        [Required]
        public decimal Faturamento { get; set; }

        [Required]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor do retorno ao contratante deve ser maior que zero.")]
        public decimal RetornoContratante { get; set; }

        [Required]
        [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O valor da fração deve ser maior ou igual a zero.")]
        public decimal ValorFracao { get; set; }

        [Required]
        [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O desconto por hora deve ser maior ou igual a zero.")]
        public decimal DescontoHora { get; set; }

        [Required]
        [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O valor mensal deve ser maior ou igual a zero.")]
        public decimal ValorMensal { get; set; }

        [Required]
        [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O valor da diária deve ser maior ou igual a zero.")]
        public decimal ValorDiaria { get; set; }

        [Required]
        [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O adicional noturno deve ser maior ou igual a zero.")]
        public decimal AdicionalNoturno { get; set; }

        public TimeSpan? HoraAbertura { get; set; }

        public TimeSpan? HoraFechamento { get; set; }

        [Required]
        [EnumDataType(typeof(TipoEstacionamento))]
        public TipoEstacionamento Tipo { get; set; }

        [Required]
        public string IdGerente { get; set; } = string.Empty;
    }

    public class EstacionamentoUpdateDto
    {
        [Required]
        public int IdEstacionamento { get; init; }

        [Required]
        [StringLength(255)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string NomeContratante { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "O número de vagas totais deve ser maior ou igual a um.")]
        public int VagasTotais { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "O número de vagas ocupadas deve ser maior ou igual a zero.")]
        public int VagasOcupadas { get; set; }

        [Required]
        public decimal Faturamento { get; set; }

        [Required]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor do retorno ao contratante deve ser maior que zero.")]
        public decimal RetornoContratante { get; set; }

        [Required]
        [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O valor da fração deve ser maior ou igual a zero.")]
        public decimal ValorFracao { get; set; }

        [Required]
        [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O desconto por hora deve ser maior ou igual a zero.")]
        public decimal DescontoHora { get; set; }

        [Required]
        [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O valor mensal deve ser maior ou igual a zero.")]
        public decimal ValorMensal { get; set; }

        [Required]
        [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O valor da diária deve ser maior ou igual a zero.")]
        public decimal ValorDiaria { get; set; }

        [Required]
        [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O adicional noturno deve ser maior ou igual a zero.")]
        public decimal AdicionalNoturno { get; set; }

        public TimeSpan? HoraAbertura { get; set; }

        public TimeSpan? HoraFechamento { get; set; }

        [Required]
        [EnumDataType(typeof(TipoEstacionamento))]
        public TipoEstacionamento Tipo { get; set; }

        [Required]
        public string IdGerente { get; set; } = string.Empty;
    }
}
