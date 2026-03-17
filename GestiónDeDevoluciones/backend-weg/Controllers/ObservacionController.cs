using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ObservacionController : ControllerBase
    {
        private readonly IObservacionService _servicioObservacion;

        public ObservacionController(IObservacionService servicioObservacion)
        {
            _servicioObservacion = servicioObservacion;
        }

        [HttpGet]
        public IActionResult Obtener()
        {
            return Ok(_servicioObservacion.ObtenerTodos());
        }

        [HttpPost]
        public IActionResult Crear([FromBody] Observaciones observacion)
        {
            _servicioObservacion.Crear(observacion);
            return Ok(observacion);
        }
    }
}
