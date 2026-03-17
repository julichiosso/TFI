using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TipoEstadoController : ControllerBase
    {
        private readonly ITipoEstadoService _servicioTipoEstado;

        public TipoEstadoController(ITipoEstadoService servicioTipoEstado)
        {
            _servicioTipoEstado = servicioTipoEstado;
        }

        [HttpGet]
        public IActionResult Obtener()
        {
            return Ok(_servicioTipoEstado.ObtenerTodos());
        }

        [HttpPost]
        public IActionResult Crear([FromBody] TipoEstado tipoEstado)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _servicioTipoEstado.Crear(tipoEstado);
            return Ok();
        }
    }
}
