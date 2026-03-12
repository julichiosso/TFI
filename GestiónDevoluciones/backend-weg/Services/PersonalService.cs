using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services
{
    public class PersonalService : IPersonalService
    {
        private readonly AppDbContext _contexto;

        public PersonalService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public IEnumerable<Personal> ObtenerTodos()
        {
            return _contexto.Personal.ToList();
        }

        public Personal? ObtenerPorId(int id)
        {
            return _contexto.Personal.Find(id);
        }

        public void Crear(Personal personal)
        {
            _contexto.Personal.Add(personal);
            _contexto.SaveChanges();
        }
    }
}
