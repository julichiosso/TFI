using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PersonalController : ControllerBase
    {
        private readonly IPersonalService _servicioPersonal;

        public PersonalController(IPersonalService servicioPersonal)
        {
            _servicioPersonal = servicioPersonal;
        }

        [HttpGet]
        public IActionResult Obtener()
        {
            return Ok(_servicioPersonal.ObtenerTodos());
        }

        [HttpPost]
        public IActionResult Crear([FromBody] Personal personal)
        {
            _servicioPersonal.Crear(personal);
            return Ok();
        }
    }
}
