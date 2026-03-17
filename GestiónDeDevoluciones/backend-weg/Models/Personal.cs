using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeDevoluciones.Models
{
    public class Personal
    {
        [Key]
        public int PersonalId { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Legajo { get; set; } = string.Empty;

        public int? UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }

        public ICollection<Observaciones> Observaciones { get; set; } = new List<Observaciones>();
        public ICollection<DecisionAdoptada> DecisionesAdoptadas { get; set; } = new List<DecisionAdoptada>();
    }
}
