using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionDeDevoluciones.Services
{
    public class RemitoService : IRemitoService
    {
        private readonly AppDbContext _contexto;

        public RemitoService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public IEnumerable<Remito> ObtenerTodos()
        {
            return _contexto.Remitos
                .Include(r => r.Usuario)
                .Include(r => r.Cliente)
                .Include(r => r.TipoEstado)
                .Include(r => r.Productos)
                .Include(r => r.Decision)
                .Include(r => r.Observacion)
                .ToList();
        }

        public Remito? ObtenerPorId(int id)
        {
            return _contexto.Remitos
                .Include(r => r.Usuario)
                .Include(r => r.Cliente)
                .Include(r => r.TipoEstado)
                .Include(r => r.Productos)
                .Include(r => r.Decision)
                .Include(r => r.Observacion)
                .FirstOrDefault(r => r.RemitoId == id);
        }

        public void Crear(Remito remito)
        {
            _contexto.Remitos.Add(remito);
            _contexto.SaveChanges();
        }

        public void Actualizar(Remito remito)
        {
            var existente = _contexto.Remitos.Find(remito.RemitoId);
            if (existente != null)
            {
                existente.ClienteId = remito.ClienteId;
                existente.TipoEstadoId = remito.TipoEstadoId;
                existente.Motivo = remito.Motivo;
                existente.ObservacionTexto = remito.ObservacionTexto;
                existente.ArchivoRemito = remito.ArchivoRemito;
                existente.ObservacionId = remito.ObservacionId;
                existente.DecisionAdoptadaId = remito.DecisionAdoptadaId;
                _contexto.SaveChanges();
            }
        }

        public void ActualizarEstado(int remitoId, int nuevoEstadoId)
        {
            var remito = _contexto.Remitos.Find(remitoId);
            if (remito != null)
            {
                remito.TipoEstadoId = nuevoEstadoId;
                _contexto.SaveChanges();
            }
        }
    }
}
