using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RolController : ControllerBase
    {
        private readonly IRolService _servicioRol;

        public RolController(IRolService servicioRol)
        {
            _servicioRol = servicioRol;
        }

        [HttpGet]
        public IActionResult Obtener()
        {
            return Ok(_servicioRol.ObtenerTodos());
        }

        [HttpPost]
        public IActionResult Crear([FromBody] Rol rol)
        {
            _servicioRol.Crear(rol);
            return Ok();
        }
    }
}
