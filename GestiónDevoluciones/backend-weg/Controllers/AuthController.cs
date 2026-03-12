using GestionDeDevoluciones.Backend.Auth;
using GestionDeDevoluciones.Backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _servicioAutenticacion;

        public AuthController(IAuthService servicioAutenticacion)
        {
            _servicioAutenticacion = servicioAutenticacion;
        }

        [HttpPost("register")]
        public IActionResult Registrar(RegisterDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var token = _servicioAutenticacion.Registrar(dto);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult IniciarSesion(LoginDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var token = _servicioAutenticacion.IniciarSesion(dto);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
