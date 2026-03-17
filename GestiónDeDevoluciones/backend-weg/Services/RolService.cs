using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services
{
    public class RolService : IRolService
    {
        private readonly AppDbContext _contexto;

        public RolService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public IEnumerable<Rol> ObtenerTodos()
        {
            return _contexto.Roles.ToList();
        }

        public void Crear(Rol rol)
        {
            _contexto.Roles.Add(rol);
            _contexto.SaveChanges();
        }
    }
}
