using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services
{
    public class TipoEstadoService : ITipoEstadoService
    {
        private readonly AppDbContext _contexto;

        public TipoEstadoService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public IEnumerable<TipoEstado> ObtenerTodos()
        {
            return _contexto.TiposEstado.ToList();
        }

        public void Crear(TipoEstado tipoEstado)
        {
            _contexto.TiposEstado.Add(tipoEstado);
            _contexto.SaveChanges();
        }
    }
}