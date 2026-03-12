using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services.Interfaces
{
    public interface IUsuarioService
    {
        IEnumerable<Usuario> ObtenerTodos();
        Usuario? ObtenerPorId(int id);
        void Crear(Usuario usuario);
        void Actualizar(int id, Usuario usuario);
        void Eliminar(int id);
    }
}
