using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotacion.Modelos;

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

        // GET: api/Resultados/eleccion-general
        [HttpGet("eleccion-general")]
        public async Task<IActionResult> GetResultadosGeneral(int idProceso, int? idProvincia = null)
        {
            try
            {
                int idDignidadUsar = 1;
                
                var dignidadesEnProceso = await _context.VotoDetalles
                    .Where(v => v.IdProceso == idProceso)
                    .Select(v => v.IdDignidad)
                    .Distinct()
                    .ToListAsync();

                if (dignidadesEnProceso.Any())
                {
                    idDignidadUsar = dignidadesEnProceso.First() ?? 1;
                }
                
                IQueryable<VotoDetalle> query = _context.VotoDetalles
                    .Where(v => v.IdProceso == idProceso && v.IdDignidad == idDignidadUsar);

                if (idProvincia.HasValue && idProvincia.Value > 0)
                {
                    query = from v in query
                            join j in _context.JuntasReceptoras on v.IdJunta equals j.Id
                            join r in _context.RecintosElectorales on j.IdRecinto equals r.Id
                            join p in _context.Parroquias on r.IdParroquia equals p.Id
                            join c in _context.Ciudades on p.IdCiudad equals c.Id
                            where c.IdProvincia == idProvincia.Value
                            select v;
                }

                var resultados = await query
                    .GroupBy(v => new { v.IdLista, v.IdTipoVoto })
                    .Select(g => new
                    {
                        IdLista = g.Key.IdLista,
                        IdTipoVoto = g.Key.IdTipoVoto,
                        TotalVotos = g.Count()
                    })
                    .ToListAsync();

                var listas = await _context.Listas
                    .Where(l => l.IdProceso == idProceso)
                    .Include(l => l.RecursosMultimedia)
                    .ToListAsync();

                var reporte = new List<object>();

                foreach(var list in listas)
                {
                    var conteo = resultados
                        .Where(r => r.IdLista == list.Id && r.IdTipoVoto == 1)
                        .Sum(r => r.TotalVotos);

                    var logo = list.RecursosMultimedia?.FirstOrDefault()?.UrlFoto;

                    reporte.Add(new
                    {
                        Lista = list.NombreLista,
                        Numero = list.NumeroLista,
                        Votos = conteo,
                        UrlLogo = logo,
                        EsValido = true
                    });
                }

                var votosBlancos = resultados
                    .Where(r => r.IdTipoVoto == 2)
                    .Sum(r => r.TotalVotos);
                
                if (votosBlancos > 0)
                {
                    reporte.Add(new { Lista = "VOTOS EN BLANCO", Numero = 0, Votos = votosBlancos, UrlLogo = "", EsValido = false });
                }

                var votosNulos = resultados
                    .Where(r => r.IdTipoVoto == 3)
                    .Sum(r => r.TotalVotos);

                if (votosNulos > 0)
                {
                    reporte.Add(new { Lista = "VOTOS NULOS", Numero = 0, Votos = votosNulos, UrlLogo = "", EsValido = false });
                }

                reporte = reporte.OrderByDescending(x => ((dynamic)x).Votos).ToList();

                return Ok(reporte);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error calculando resultados: " + ex.Message);
            }
        }

        // GET: api/Resultados/consulta-popular
        [HttpGet("consulta-popular")]
        public async Task<IActionResult> GetResultadosConsulta(int idProceso)
        {
            try
            {
                var resultados = await _context.VotoDetalles
                    .Where(v => v.IdProceso == idProceso && v.IdTipoVoto == 1 && v.IdPregunta != null && v.IdOpcion != null)
                    .GroupBy(v => new { v.IdPregunta, v.IdOpcion })
                    .Select(g => new
                    {
                        IdPregunta = g.Key.IdPregunta,
                        IdOpcion = g.Key.IdOpcion,
                        TotalVotos = g.Count()
                    })
                    .ToListAsync();

                var preguntas = await _context.PreguntasConsultas.Where(p => p.IdProceso == idProceso).ToListAsync();
                var opciones = await _context.OpcionConsultas.ToListAsync();

                var reporte = from r in resultados
                              join p in preguntas on r.IdPregunta equals p.Id
                              join o in opciones on r.IdOpcion equals o.Id
                              select new
                              {
                                  Pregunta = p.TextoPregunta,
                                  Opcion = o.TextoOpcion,
                                  Votos = r.TotalVotos
                              };

                var jerarquico = reporte.GroupBy(x => x.Pregunta)
                    .Select(g => new
                    {
                        Pregunta = g.Key,
                        Resultados = g.Select(x => new { Opcion = x.Opcion, Votos = x.Votos }).ToList()
                    });

                return Ok(jerarquico);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error calculando resultados: " + ex.Message);
            }
        }

        // GET: api/Resultados/auditoria
        [HttpGet("auditoria")]
        public async Task<IActionResult> GetDatosAuditoria(int idProceso, int idJunta)
        {
            try
            {
                var queryBase = _context.Padrones
                    .Include(p => p.Votante)
                    .Where(p => p.IdProceso == idProceso && p.Votante.IdJunta == idJunta);

                int totalEmpadronados = await queryBase.CountAsync();

                int totalSufragantes = await queryBase
                    .Where(p => p.HaVotado == true)
                    .CountAsync();

                return Ok(new 
                { 
                    TotalEmpadronados = totalEmpadronados,
                    TotalSufragantes = totalSufragantes
                });
            }
            catch (Exception ex)
            {
                 return StatusCode(500, "Error obteniendo datos auditoria: " + ex.Message);
            }
        }
        // DEBUG: api/Resultados/debug
        [HttpGet("debug")]
        public async Task<IActionResult> GetDebugStats(int idProceso)
        {
            var stats = await _context.VotoDetalles
                .Where(v => v.IdProceso == idProceso)
                .GroupBy(v => new { v.IdDignidad, v.IdTipoVoto, v.IdLista })
                .Select(g => new
                {
                    Dignidad = g.Key.IdDignidad,
                    TipoVoto = g.Key.IdTipoVoto,
                    Lista = g.Key.IdLista,
                    Cantidad = g.Count()
                })
                .ToListAsync();

            var listasEnProceso = await _context.Listas
                .Where(l => l.IdProceso == idProceso)
                .Select(l => new { l.Id, l.NombreLista })
                .ToListAsync();
            
            return Ok(new 
            { 
                ProcesoId = idProceso, 
                TotalVotosEnProceso = stats.Sum(x => x.Cantidad),
                ListasEncontradas = listasEnProceso.Count,
                DetalleListas = listasEnProceso,
                DetalleVotos = stats 
            });
        }
    }
}
