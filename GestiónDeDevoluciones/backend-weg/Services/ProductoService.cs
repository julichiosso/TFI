using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Data;
using GestionDeDevoluciones.Models;

namespace GestionDeDevoluciones.Services
{
    public class ProductoService : IProductoService
    {
        private readonly AppDbContext _contexto;

        public ProductoService(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        public IEnumerable<Producto> ObtenerTodos()
        {
            return _contexto.Productos.ToList();
        }

        public Producto? ObtenerPorId(int id)
        {
            return _contexto.Productos.Find(id);
        }

        public void Crear(Producto producto)
        {
            _contexto.Productos.Add(producto);
            _contexto.SaveChanges();
        }

        public void Actualizar(Producto producto)
        {
            _contexto.Productos.Update(producto);
            _contexto.SaveChanges();
        }

        public void Eliminar(int id)
        {
            var producto = _contexto.Productos.Find(id);
            if (producto != null)
            {
                _contexto.Productos.Remove(producto);
                _contexto.SaveChanges();
            }
        }
    }
}
