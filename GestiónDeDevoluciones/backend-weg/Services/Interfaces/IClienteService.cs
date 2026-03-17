using GestionDeDevoluciones.Models;
using System.Collections.Generic;

namespace GestionDeDevoluciones.Services.Interfaces
{
    public interface IClienteService
    {
        IEnumerable<Cliente> ObtenerTodos();
        Cliente? ObtenerPorId(int id);
        void Crear(Cliente cliente);
        void Actualizar(int id, Cliente cliente);
        void Eliminar(int id);
    }
}