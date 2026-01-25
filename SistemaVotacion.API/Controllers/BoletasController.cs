using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotacion.Modelos;

namespace SistemaVotacion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoletasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public BoletasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet("activas")]
        public IActionResult ObtenerBoletaActiva()
        {
            var ahora = DateTime.Now;

            var procesoActivo = _context.ProcesosElectorales
                .Include(p => p.ListasParticipantes)
                .ThenInclude(l => l.Candidatos)
                .ThenInclude(c => c.Dignidad)
                .Include(p => p.ListasParticipantes)
                .ThenInclude(l => l.Candidatos)
                .ThenInclude(c => c.GaleriaMultimedia)
                .Include(p => p.ListasParticipantes)
                .ThenInclude(l => l.RecursosMultimedia)
                .FirstOrDefault(p =>
                    ahora >= p.FechaInicio &&
                    ahora <= p.FechaFin
                );

            if (procesoActivo == null)
                return NotFound("No hay proceso electoral activo");

            return Ok(procesoActivo.ListasParticipantes);
        }
    }
}