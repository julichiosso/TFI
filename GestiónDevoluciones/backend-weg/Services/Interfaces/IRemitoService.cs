using System.Collections.Generic;
using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services.Interfaces
{
    public interface IRemitoService
    {
        IEnumerable<Remito> ObtenerTodos();
        Remito? ObtenerPorId(int id);
        void Crear(Remito remito);
        void Actualizar(Remito remito);
        void ActualizarEstado(int remitoId, int nuevoEstadoId);
    }
}
