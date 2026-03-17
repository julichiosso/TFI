using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;
using GestionDeDevoluciones.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GestionDeDevoluciones.Services
{
    public class ClienteService : IClienteService
    {
        private readonly AppDbContext _contexto;

        public ClienteService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public IEnumerable<Cliente> ObtenerTodos()
        {
            return _contexto.Clientes
                .Include(c => c.Direccion)
                .ToList();
        }

        public Cliente? ObtenerPorId(int id)
        {
            return _contexto.Clientes
                .Include(c => c.Direccion)
                .FirstOrDefault(c => c.ClienteId == id);
        }

        public void Crear(Cliente cliente)
        {
            if (cliente.Direccion != null)
            {
                bool direccionVacia =
                    string.IsNullOrWhiteSpace(cliente.Direccion.Calle) &&
                    string.IsNullOrWhiteSpace(cliente.Direccion.Numero) &&
                    string.IsNullOrWhiteSpace(cliente.Direccion.Ciudad);

                if (direccionVacia)
                {
                    cliente.Direccion = null;
                    cliente.DireccionId = null;
                }
            }

            _contexto.Clientes.Add(cliente);
            _contexto.SaveChanges();
        }

        public void Actualizar(int id, Cliente cliente)
        {
            var existente = _contexto.Clientes
                .Include(c => c.Direccion)
                .FirstOrDefault(c => c.ClienteId == id);

            if (existente != null)
            {
                existente.RazonSocial = cliente.RazonSocial;
                existente.CodigoCliente = cliente.CodigoCliente;

                if (cliente.Direccion != null)
                {
                    if (existente.Direccion == null)
                    {
                        existente.Direccion = new Direccion();
                    }
                    existente.Direccion.Calle = cliente.Direccion.Calle;
                    existente.Direccion.Numero = cliente.Direccion.Numero;
                    existente.Direccion.Ciudad = cliente.Direccion.Ciudad;
                }

                _contexto.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            var cliente = _contexto.Clientes.Find(id);
            if (cliente != null)
            {
                _contexto.Clientes.Remove(cliente);
                _contexto.SaveChanges();
            }
        }
    }
}