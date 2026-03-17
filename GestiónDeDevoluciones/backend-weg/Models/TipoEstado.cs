using System.ComponentModel.DataAnnotations;

namespace GestionDeDevoluciones.Models
{
    public class TipoEstado
    {
        [Key]
        public int TipoEstadoId { get; set; }

        [Required]
        public string Estado { get; set; } = string.Empty;
    }
}
