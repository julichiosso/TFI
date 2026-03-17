using GestionDeDevoluciones.Services.Interfaces;
using GestionDeDevoluciones.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DecisionAdoptadaController : ControllerBase
    {
        private readonly IDecisionAdoptadaService _servicioDecision;

        public DecisionAdoptadaController(IDecisionAdoptadaService servicioDecision)
        {
            _servicioDecision = servicioDecision;
        }

        [HttpGet]
        public IActionResult Obtener()
        {
            return Ok(_servicioDecision.ObtenerTodos());
        }

        [HttpPost]
        public IActionResult Crear([FromBody] DecisionAdoptada decision)
        {
            _servicioDecision.Crear(decision);
            return Ok(decision);
        }
    }
}
