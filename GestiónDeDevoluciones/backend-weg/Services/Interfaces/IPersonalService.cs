using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services.Interfaces
{
    public interface IPersonalService
    {
        IEnumerable<Personal> ObtenerTodos();
        Personal? ObtenerPorId(int id);
        void Crear(Personal personal);
    }
}
