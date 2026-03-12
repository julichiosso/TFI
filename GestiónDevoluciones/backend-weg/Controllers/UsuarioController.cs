using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _servicio;

        public UsuarioController(IUsuarioService servicio)
        {
            _servicio = servicio;
        }

        [HttpGet]
        public IActionResult Obtener()
        {
            return Ok(_servicio.ObtenerTodos());
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var usuario = _servicio.ObtenerPorId(id);
            if (usuario == null) return NotFound();
            return Ok(usuario);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] Usuario usuario)
        {
            if (string.IsNullOrEmpty(usuario.ContrasenaHash))
            {
                return BadRequest(new { error = "La contraseña es obligatoria para nuevos usuarios." });
            }

            if (!ModelState.IsValid) 
            {
                var errores = string.Join(". ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new { error = errores });
            }

            try 
            {
                _servicio.Crear(usuario);
                return Ok(usuario);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid) 
            {
                var errores = string.Join(". ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new { error = errores });
            }

            try
            {
                _servicio.Actualizar(id, usuario);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            try
            {
                _servicio.Eliminar(id);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
