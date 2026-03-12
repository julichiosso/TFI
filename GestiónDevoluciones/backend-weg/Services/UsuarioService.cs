using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _contexto;

        public UsuarioService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public IEnumerable<Usuario> ObtenerTodos()
        {
            return _contexto.Usuarios.ToList();
        }

        public Usuario? ObtenerPorId(int id)
        {
            return _contexto.Usuarios.Find(id);
        }

        public void Crear(Usuario usuario)
        {
            if (!string.IsNullOrEmpty(usuario.ContrasenaHash))
            {
                usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(usuario.ContrasenaHash);
            }
            _contexto.Usuarios.Add(usuario);
            _contexto.SaveChanges();
        }

        public void Actualizar(int id, Usuario usuario)
        {
            var existente = _contexto.Usuarios.Find(id);
            if (existente != null)
            {
                existente.Nombre = usuario.Nombre;
                existente.Email = usuario.Email;
                existente.RolId = usuario.RolId;
                if (!string.IsNullOrEmpty(usuario.ContrasenaHash))
                {
                    existente.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(usuario.ContrasenaHash);
                }
                _contexto.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            var usuario = _contexto.Usuarios.Find(id);
            if (usuario != null)
            {
                _contexto.Usuarios.Remove(usuario);
                _contexto.SaveChanges();
            }
        }
    }
}
