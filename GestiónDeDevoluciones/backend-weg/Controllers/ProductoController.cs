using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _servicioProducto;

        public ProductoController(IProductoService servicioProducto)
        {
            _servicioProducto = servicioProducto;
        }

        [HttpGet]
        public IActionResult Obtener()
        {
            return Ok(_servicioProducto.ObtenerTodos());
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var producto = _servicioProducto.ObtenerPorId(id);
            if (producto == null) return NotFound();
            return Ok(producto);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] Producto producto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _servicioProducto.Crear(producto);
            return Ok(producto);
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] Producto producto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != producto.ProductoId) return BadRequest();
            _servicioProducto.Actualizar(producto);
            return Ok(producto);
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            _servicioProducto.Eliminar(id);
            return Ok();
        }
    }
}
