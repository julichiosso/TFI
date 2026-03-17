using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeDevoluciones.Models
{
    public class Observaciones
    {
        [Key]
        public int ObservacionId { get; set; }

        [Required]
        public int PersonalId { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required, StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        [ForeignKey(nameof(PersonalId))]
        public Personal? Personal { get; set; } = null!;
    }
}
