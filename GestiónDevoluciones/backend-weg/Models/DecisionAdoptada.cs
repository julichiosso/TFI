namespace GestionDeDevoluciones.Models
{
    public class DecisionAdoptada
    {
        public int DecisionAdoptadaId { get; set; }

        public int PersonalId { get; set; }
        public Personal? Personal { get; set; } = null!;

        public DateTime Fecha { get; set; }

        public string Descripcion { get; set; } = string.Empty;
    }
}
