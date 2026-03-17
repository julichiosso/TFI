using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services.Interfaces
{
    public interface IObservacionService
    {
        IEnumerable<Observaciones> ObtenerTodos();
        void Crear(Observaciones observacion);
    }
}
