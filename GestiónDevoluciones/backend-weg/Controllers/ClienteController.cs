using GestionDeDevoluciones.Models;
using GestionDeDevoluciones.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _servicioCliente;

        public ClienteController(IClienteService servicioCliente)
        {
            _servicioCliente = servicioCliente;
        }

        [HttpGet]
        public IActionResult Obtener()
        {
            return Ok(_servicioCliente.ObtenerTodos());
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var cliente = _servicioCliente.ObtenerPorId(id);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] Cliente cliente)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _servicioCliente.Crear(cliente);
            return Ok(cliente);
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] Cliente cliente)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _servicioCliente.Actualizar(id, cliente);
            return Ok(cliente);
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            _servicioCliente.Eliminar(id);
            return NoContent();
        }
    }
}
