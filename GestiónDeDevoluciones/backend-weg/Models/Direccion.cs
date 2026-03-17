using System.ComponentModel.DataAnnotations;

namespace GestionDeDevoluciones.Models
{
    public class Direccion
    {
        [Key]
        public int DireccionId { get; set; }

        [StringLength(100)]
        public string? Calle { get; set; }

        [StringLength(10)]
        public string? Numero { get; set; }

        [StringLength(80)]
        public string? Ciudad { get; set; }
    }
}