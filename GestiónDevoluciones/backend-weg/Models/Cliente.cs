using System.ComponentModel.DataAnnotations;

namespace GestionDeDevoluciones.Models
{
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }

        [Required, StringLength(150)]
        public string RazonSocial { get; set; } = string.Empty;

        [StringLength(50)]
        public string? CodigoCliente { get; set; }

        public int? DireccionId { get; set; }

        public Direccion? Direccion { get; set; }
    }
}