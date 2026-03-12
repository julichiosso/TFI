using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services
{
    public class ObservacionService : IObservacionService
    {
        private readonly AppDbContext _contexto;

        public ObservacionService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public IEnumerable<Observaciones> ObtenerTodos()
        {
            return _contexto.Observaciones.ToList();
        }

        public void Crear(Observaciones observacion)
        {
            _contexto.Observaciones.Add(observacion);
            _contexto.SaveChanges();
        }
    }
}
