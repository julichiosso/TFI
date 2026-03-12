using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services.Interfaces
{
    public interface IProductoService
    {
        IEnumerable<Producto> ObtenerTodos();
        Producto? ObtenerPorId(int id);
        void Crear(Producto producto);
        void Actualizar(Producto producto);
        void Eliminar(int id);
    }
}
