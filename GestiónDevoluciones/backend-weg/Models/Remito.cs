using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;

namespace GestionDeDevoluciones.Models
{
    public class Remito
    {
        public int RemitoId { get; set; }

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int TipoEstadoId { get; set; }
        public TipoEstado? TipoEstado { get; set; }

        public int? DecisionAdoptadaId { get; set; }
        [ForeignKey(nameof(DecisionAdoptadaId))]
        public DecisionAdoptada? Decision { get; set; }

        public int? ObservacionId { get; set; }
        [ForeignKey(nameof(ObservacionId))]
        public Observaciones? Observacion { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        [StringLength(200)]
        public string? Motivo { get; set; }

        [StringLength(500)]
        public string? ObservacionTexto { get; set; }

        [StringLength(300)]
        public string? ArchivoRemito { get; set; }

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
