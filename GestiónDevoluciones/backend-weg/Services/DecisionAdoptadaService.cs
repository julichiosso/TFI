using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services
{
    public class DecisionAdoptadaService : IDecisionAdoptadaService
    {
        private readonly AppDbContext _contexto;

        public DecisionAdoptadaService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public IEnumerable<DecisionAdoptada> ObtenerTodos()
        {
            return _contexto.DecisionesAdoptadas.ToList();
        }

        public void Crear(DecisionAdoptada decision)
        {
            _contexto.DecisionesAdoptadas.Add(decision);
            _contexto.SaveChanges();
        }
    }
}
