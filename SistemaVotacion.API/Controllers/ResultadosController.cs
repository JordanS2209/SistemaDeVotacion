using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotacion.API.DTOs;


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
        [HttpGet("consulta-popular")]
        public async Task<IActionResult> ObtenerResultadosConsultaPopular()
        {
            var ahora = DateTime.Now;

            var proceso = await _context.ProcesosElectorales
                .FirstOrDefaultAsync(p =>
                    ahora >= p.FechaInicio &&
                    ahora <= p.FechaFin &&
                    p.IdTipoProceso == 2);

            if (proceso == null)
                return BadRequest("No hay consulta popular activa");

            var preguntas = await _context.PreguntasConsultas
                .Where(p => p.IdProceso == proceso.Id)
                .ToListAsync();

            var resultados = new List<ResultadoPreguntaDto>();

            foreach (var pregunta in preguntas)
            {
                var votos = await _context.VotoDetalles
                    .Where(v =>
                        v.IdProceso == proceso.Id &&
                        v.IdPregunta == pregunta.Id)
                    .Include(v => v.Opcion)
                    .Include(v => v.TipoVoto)
                    .ToListAsync();

                var totalSi = votos.Count(v =>
                    (v.Opcion?.TextoOpcion ?? string.Empty).ToUpper() == "SÍ");

                var totalNo = votos.Count(v =>
                    (v.Opcion?.TextoOpcion ?? string.Empty).ToUpper() == "NO");

                var totalBlanco = votos.Count(v =>
                    (v.TipoVoto?.NombreTipo ?? string.Empty).ToUpper().Contains("BLANCO"));

                var ganador =
                    totalSi > totalNo ? "SÍ" :
                    totalNo > totalSi ? "NO" :
                    "EMPATE";

                resultados.Add(new ResultadoPreguntaDto
                {
                    IdPregunta = pregunta.Id,
                    TextoPregunta = pregunta.TextoPregunta,
                    TotalSi = totalSi,
                    TotalNo = totalNo,
                    TotalBlancos = totalBlanco,
                    Ganador = ganador
                });
            }

            var dto = new ResultadoConsultaDto
            {
                IdProceso = proceso.Id,
                NombreProceso = proceso.NombreProceso,
                Resultados = resultados
            };

            return Ok(dto);
        }

        [HttpGet("eleccion-general")]
        public async Task<IActionResult> ObtenerResultadosEleccionGeneral()
        {
            var ahora = DateTime.Now;

            var proceso = await _context.ProcesosElectorales
                .FirstOrDefaultAsync(p =>
                    ahora >= p.FechaInicio &&
                    ahora <= p.FechaFin &&
                    p.IdTipoProceso == 1);

            if (proceso == null)
                return BadRequest("No hay elección general activa");

            var listas = await _context.VotoDetalles
                .Where(v =>
                    v.IdProceso == proceso.Id &&
                    v.IdLista != null)
                .GroupBy(v => new { v.IdLista, v.Lista.NombreLista })
                .Select(g => new ResultadoListaDto
                {
                    IdLista = g.Key.IdLista.Value,
                    NombreLista = g.Key.NombreLista,
                    TotalVotos = g.Count()
                })
                .OrderByDescending(x => x.TotalVotos)
                .ToListAsync();

            var totalVotos = listas.Sum(l => l.TotalVotos);

            var ganador = listas.FirstOrDefault()?.NombreLista ?? "Sin votos";

            var dto = new ResultadoEleccionDto
            {
                IdProceso = proceso.Id,
                NombreProceso = proceso.NombreProceso,
                TotalVotos = totalVotos,
                Ganador = ganador,
                Resultados = listas
            };

            return Ok(dto);
        }

    }
}
