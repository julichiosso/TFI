using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services.Interfaces
{
    public interface IDecisionAdoptadaService
    {
        IEnumerable<DecisionAdoptada> ObtenerTodos();
        void Crear(DecisionAdoptada decision);
    }
}
