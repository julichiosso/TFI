using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GestionDeDevoluciones.Models
{
    public class Producto
    {
        [Key]
        public int ProductoId { get; set; }

        public int RemitoId { get; set; }

        [StringLength(150)]
        public string? Item { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Cantidad { get; set; }

        [StringLength(80)]
        [JsonPropertyName("cv")]
        public string? CV { get; set; }

        [StringLength(80)]
        [JsonPropertyName("rpm")]
        public string? RPM { get; set; }

        [StringLength(50)]
        public string? Voltaje { get; set; }

        [StringLength(120)]
        [JsonPropertyName("protec")]
        public string? Proteccion { get; set; }

        [StringLength(120)]
        [JsonPropertyName("const")]
        public string? Construccion { get; set; }

        [Required(ErrorMessage = "El modelo es obligatorio"), StringLength(120)]
        public string? Modelo { get; set; }

        [Required(ErrorMessage = "El número de serie es obligatorio"), StringLength(100)]
        public string? NumeroSerie { get; set; }

        [ForeignKey(nameof(RemitoId))]
        public Remito? Remito { get; set; }
    }
}
