using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaVotacion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultadosController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public ResultadosController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // ===============================
        // RESUMEN GENERAL
        // ===============================
        [HttpGet("resumen-general")]
        public async Task<IActionResult> ObtenerResumenGeneral()
        {
            var ahora = DateTime.Now;

            var proceso = await _context.ProcesosElectorales
                .FirstOrDefaultAsync(p => ahora >= p.FechaInicio && ahora <= p.FechaFin);

            if (proceso == null)
                return BadRequest("No hay proceso activo");

            var listas = await _context.VotoDetalles
                .Where(v => v.IdProceso == proceso.Id && v.IdLista != null)
                .GroupBy(v => v.Lista.NombreLista)
                .Select(g => new
                {
                    lista = g.Key,
                    votos = g.Count()
                })
                .ToListAsync();

            var tipos = await _context.VotoDetalles
                .Where(v => v.IdProceso == proceso.Id)
                .GroupBy(v => v.TipoVoto.NombreTipo)
                .Select(g => new
                {
                    tipo = g.Key.ToUpper(),
                    total = g.Count()
                })
                .ToListAsync();

            var validos = tipos.FirstOrDefault(t => t.tipo.Contains("VALIDO"))?.total ?? 0;
            var blancos = tipos.FirstOrDefault(t => t.tipo.Contains("BLANCO"))?.total ?? 0;

            return Ok(new
            {
                listas,
                resumen = new { validos, blancos }
            });
        }

        [HttpGet("por-provincia/{idProvincia}")]
        public async Task<IActionResult> ObtenerPorProvincia(int idProvincia)
        {
            var ahora = DateTime.Now;

            var proceso = await _context.ProcesosElectorales
                .FirstOrDefaultAsync(p => ahora >= p.FechaInicio && ahora <= p.FechaFin);

            if (proceso == null)
                return BadRequest("No hay proceso activo");

            var resultados = await (
                    from v in _context.VotoDetalles
                    join j in _context.JuntasReceptoras
                        on v.IdJunta equals j.Id
                    join r in _context.RecintosElectorales
                        on j.IdRecinto equals r.Id
                    join p in _context.Parroquias
                        on r.IdParroquia equals p.Id
                    join c in _context.Ciudades
                        on p.IdCiudad equals c.Id
                    where
                        v.IdProceso == proceso.Id &&
                        v.IdLista != null &&
                        c.IdProvincia == idProvincia
                    group v by v.Lista.NombreLista into g
                    select new
                    {
                        lista = g.Key,
                        votos = g.Count()
                    }
                )
                .OrderByDescending(x => x.votos)
                .ToListAsync();

            return Ok(resultados);
        }

    }
}
