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

        // NUEVO: Obtener boleta validando código de acceso (urna)
        [HttpGet("por-codigo/{codigo}")]
        public async Task<IActionResult> ObtenerBoletaPorCodigo(string codigo)
        {
            var ahora = DateTime.Now;

            var padron = await _context.Padrones
                .Include(p => p.Proceso)
                .FirstOrDefaultAsync(p =>
                    p.CodigoAcceso == codigo &&
                    !p.HaVotado &&
                    ahora >= p.Proceso.FechaInicio &&
                    ahora <= p.Proceso.FechaFin);

            if (padron == null)
            {
                return BadRequest("Código inválido o ya utilizado.");
            }

            var proceso = await _context.ProcesosElectorales
                .Include(p => p.ListasParticipantes)
                    .ThenInclude(l => l.Candidatos)
                        .ThenInclude(c => c.Dignidad)
                .Include(p => p.ListasParticipantes)
                    .ThenInclude(l => l.Candidatos)
                        .ThenInclude(c => c.GaleriaMultimedia)
                .Include(p => p.ListasParticipantes)
                    .ThenInclude(l => l.RecursosMultimedia)
                .FirstOrDefaultAsync(p => p.Id == padron.IdProceso);

            if (proceso == null)
            {
                return NotFound("No se encontró el proceso electoral para el código dado.");
            }

            // Opcional: no invalidamos aún el código hasta que efectivamente vote
            return Ok(new
            {
                PadronId = padron.Id, // para usarlo al registrar el voto
                Listas = proceso.ListasParticipantes
            });
        }
    }
}

