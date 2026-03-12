using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RemitoController : ControllerBase
    {
        private readonly IRemitoService _servicioRemito;
        private readonly IWebHostEnvironment _entorno;

        public RemitoController(IRemitoService servicioRemito, IWebHostEnvironment entorno)
        {
            _servicioRemito = servicioRemito;
            _entorno = entorno;
        }

        [HttpGet]
        public IActionResult Obtener()
        {
            try 
            {
                var remitos = _servicioRemito.ObtenerTodos();
                return Ok(remitos);
            }
            catch (System.Exception ex)
            {
                var mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"ERROR AL OBTENER REMITOS: {mensajeError}");
                return StatusCode(500, new { error = mensajeError });
            }
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var remito = _servicioRemito.ObtenerPorId(id);
            if (remito == null) return NotFound();
            return Ok(remito);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] Remito remito)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try 
            {
                if (remito == null) return BadRequest("El remito no puede ser nulo.");
                if (remito.Fecha == default) remito.Fecha = System.DateTime.Now;

                _servicioRemito.Crear(remito);
                return Ok(new { remitoId = remito.RemitoId });
            }
            catch (System.Exception ex)
            {
                var mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"ERROR AL GUARDAR REMITO: {mensajeError}");
                return StatusCode(500, new { error = mensajeError });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] Remito remito)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                remito.RemitoId = id;
                _servicioRemito.Actualizar(remito);
                return Ok();
            }
            catch (System.Exception ex)
            {
                var mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { error = mensajeError });
            }
        }

        [HttpPut("{id}/estado")]
        public IActionResult ActualizarEstado(int id, [FromBody] int nuevoEstadoId)
        {
            _servicioRemito.ActualizarEstado(id, nuevoEstadoId);
            return Ok();
        }

        [HttpPost("subir")]
        public async Task<IActionResult> SubirArchivo(IFormFile archivo)
        {
            try
            {
                if (archivo == null || archivo.Length == 0)
                    return BadRequest("No se recibió ningún archivo.");

                var extension = Path.GetExtension(archivo.FileName).ToLower();
                if (extension != ".pdf" && extension != ".png" && extension != ".jpg" && extension != ".jpeg")
                    return BadRequest("Solo se permiten archivos PDF, PNG o JPG.");

                var carpetaSubidas = Path.Combine(_entorno.ContentRootPath, "Uploads");
                if (!Directory.Exists(carpetaSubidas))
                    Directory.CreateDirectory(carpetaSubidas);

                var nombreUnico = $"{Guid.NewGuid()}{extension}";
                var rutaArchivo = Path.Combine(carpetaSubidas, nombreUnico);

                using (var flujo = new FileStream(rutaArchivo, FileMode.Create))
                {
                    await archivo.CopyToAsync(flujo);
                }

                return Ok(new { fileName = nombreUnico, filePath = $"/uploads/{nombreUnico}" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
