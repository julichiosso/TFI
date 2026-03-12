using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services.Interfaces
{
    public interface ITipoEstadoService
    {
        IEnumerable<TipoEstado> ObtenerTodos();
        void Crear(TipoEstado tipoEstado);
    }
}
