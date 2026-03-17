using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services.Interfaces
{
    public interface IRolService
    {
        IEnumerable<Rol> ObtenerTodos();
        void Crear(Rol rol);
    }
}
