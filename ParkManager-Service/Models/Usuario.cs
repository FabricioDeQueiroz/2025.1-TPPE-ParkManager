using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ParkManager_Service.Models;

public enum TipoUsuario
{
    Gerente,
    Cliente
}

[Table("USUARIO")]
public class Usuario
{
    [Key]
    [Column("id_usuario")]
    public int IdUsuario { get; init; }

    [Required]
    [StringLength(255)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    [Column("senha")]
    public string Senha { get; set; } = string.Empty;

    [Required]
    [Column("tipo")]
    [EnumDataType(typeof(TipoUsuario))]
    public TipoUsuario Tipo { get; set; }

    public ICollection<Estacionamento> EstacionamentosGerenciados { get; } = new List<Estacionamento>();
    public ICollection<Acesso> Acessos { get; } = new List<Acesso>();
}
