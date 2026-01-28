using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotacion.Modelos;

namespace SistemaVotacion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PadronesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public PadronesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Padron>>>> GetPadrones()
        {
            try
            {
                var padrones = await _context.Padrones.ToListAsync();
                return ApiResult<List<Padron>>.Ok(padrones);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Padron>>.Fail(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Padron>>> GetPadron(int id)
        {
            try
            {
                var padron = await _context.Padrones
                    .Include(p => p.Proceso)
                    .Include(p => p.Votante)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (padron == null)
                {
                    return ApiResult<Padron>.Fail("Padron no encontrado.");
                }

                return ApiResult<Padron>.Ok(padron);
            }
            catch (Exception ex)
            {
                return ApiResult<Padron>.Fail(ex.Message);
            }
        }

        
        [HttpGet("estado-votante-identificacion/{numeroIdentificacion}")]
        public async Task<IActionResult> ObtenerEstadoPorIdentificacion(string numeroIdentificacion)
        {
            var padron = await _context.Padrones
                .Include(p => p.Votante)
                .ThenInclude(v => v.Usuario)
                .Include(p => p.Proceso)
                .FirstOrDefaultAsync(p =>
                    p.Votante.Usuario.NumeroIdentificacion == numeroIdentificacion
                );

            if (padron == null)
            {
                return NotFound("El votante no se encuentra en el padrón.");
            }

            var procesoActivo = false;
            if (padron.Proceso != null)
            {
                procesoActivo =
                    DateTime.Now >= padron.Proceso.FechaInicio &&
                    DateTime.Now <= padron.Proceso.FechaFin;
            }

            return Ok(new
            {
                PadronId = padron.Id,
                HaVotado = padron.HaVotado,
                CodigoAcceso = padron.CodigoAcceso,
                ProcesoActivo = procesoActivo
            });
        }

        // NUEVO: Buscar por cédula, crear padrón si no existe y generar código
        [HttpPost("crear-o-generar-codigo/{numeroIdentificacion}")]
        public async Task<IActionResult> CrearPadronYGenerarCodigo(string numeroIdentificacion)
        {
            var padron = await _context.Padrones
                .Include(p => p.Votante)
                .ThenInclude(v => v.Usuario)
                .Include(p => p.Proceso)
                .FirstOrDefaultAsync(p =>
                    p.Votante.Usuario.NumeroIdentificacion == numeroIdentificacion
                );

            if (padron == null)
            {
                // Buscar votante por cédula
                var votante = await _context.Votantes
                    .Include(v => v.Usuario)
                    .FirstOrDefaultAsync(v => v.Usuario.NumeroIdentificacion == numeroIdentificacion);

                if (votante == null)
                {
                    return NotFound("No existe un votante con esa identificación.");
                }

                // Buscar ÚNICO proceso activo
                var ahora = DateTime.Now;
                var procesoActivo = await _context.ProcesosElectorales
                    .FirstOrDefaultAsync(p =>
                        ahora >= p.FechaInicio &&
                        ahora <= p.FechaFin
                    );

                if (procesoActivo == null)
                {
                    return BadRequest("No hay proceso electoral activo.");
                }

                padron = new Padron
                {
                    IdVotante = votante.Id,
                    IdProceso = procesoActivo.Id,
                    HaVotado = false
                };

                padron.CodigoAcceso = new Random().Next(100000, 999999).ToString();

                _context.Padrones.Add(padron);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    PadronId = padron.Id,
                    HaVotado = padron.HaVotado,
                    CodigoAcceso = padron.CodigoAcceso,
                    ProcesoActivo = true
                });
            }


            if (padron.Proceso == null)
            {
                var ahora = DateTime.Now;
                var procesoActivo = await _context.ProcesosElectorales
                    .FirstOrDefaultAsync(p =>
                        ahora >= p.FechaInicio &&
                        ahora <= p.FechaFin
                    );

                if (procesoActivo == null)
                {
                    return BadRequest("No hay proceso electoral activo.");
                }

                padron.IdProceso = procesoActivo.Id;
                padron.Proceso = procesoActivo;
                await _context.SaveChangesAsync();
            }

            var procesoEstaActivo =
                DateTime.Now >= padron.Proceso.FechaInicio &&
                DateTime.Now <= padron.Proceso.FechaFin;

            if (!procesoEstaActivo)
            {
                return BadRequest("El proceso electoral no está activo.");
            }

            if (string.IsNullOrWhiteSpace(padron.CodigoAcceso))
            {
                padron.CodigoAcceso = new Random().Next(100000, 999999).ToString();
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                PadronId = padron.Id,
                HaVotado = padron.HaVotado,
                CodigoAcceso = padron.CodigoAcceso,
                ProcesoActivo = procesoEstaActivo
            });
        }

        [HttpPost("habilitar/{padronId}")]
        public async Task<IActionResult> HabilitarVotacion(int padronId)
        {
            var padron = await _context.Padrones
                .Include(p => p.Proceso)
                .FirstOrDefaultAsync(p => p.Id == padronId);

            if (padron == null)
            {
                return NotFound("Padrón no encontrado.");
            }

            if (padron.HaVotado)
            {
                return BadRequest("El votante ya sufragó.");
            }

            if (padron.Proceso == null)
            {
                return BadRequest("El padrón no tiene un proceso electoral asociado.");
            }

            if (DateTime.Now < padron.Proceso.FechaInicio ||
                DateTime.Now > padron.Proceso.FechaFin)
            {
                return BadRequest("El proceso electoral no está activo.");
            }

           
            if (string.IsNullOrWhiteSpace(padron.CodigoAcceso))
            {
                padron.CodigoAcceso = new Random().Next(100000, 999999).ToString();
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                padron.Id,
                padron.CodigoAcceso
            });
        }

        // ==================================================

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Padron>>> PutPadron(int id, Padron padron)
        {
            if (id != padron.Id)
            {
                return ApiResult<Padron>.Fail("ID de Padron no coincide.");
            }

            _context.Entry(padron).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PadronExists(id))
                {
                    return ApiResult<Padron>.Fail("Padron no encontrado.");
                }
                else
                {
                    return ApiResult<Padron>.Fail(ex.Message);
                }
            }

            return ApiResult<Padron>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Padron>>> PostPadron(Padron padron)
        {
            try
            {
                _context.Padrones.Add(padron);
                await _context.SaveChangesAsync();
                return ApiResult<Padron>.Ok(padron);
            }
            catch (Exception ex)
            {
                return ApiResult<Padron>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Padron>>> DeletePadron(int id)
        {
            try
            {
                var padron = await _context.Padrones.FindAsync(id);
                if (padron == null)
                {
                    return ApiResult<Padron>.Fail("Padron no encontrado.");
                }

                _context.Padrones.Remove(padron);
                await _context.SaveChangesAsync();

                return ApiResult<Padron>.Ok(padron);
            }
            catch (Exception ex)
            {
                return ApiResult<Padron>.Fail(ex.Message);
            }
        }

        
        [HttpGet("validar-codigo/{codigoAcceso}")]
        public async Task<IActionResult> ValidarCodigoAcceso(string codigoAcceso)
        {
            var ahora = DateTime.Now;

            var padron = await _context.Padrones
                .Include(p => p.Proceso)
                .FirstOrDefaultAsync(p => p.CodigoAcceso == codigoAcceso);

            if (padron == null)
            {
                return NotFound("Código no válido.");
            }

            if (padron.HaVotado)
            {
                return BadRequest("El votante ya sufragó.");
            }

            if (padron.Proceso == null ||
                ahora < padron.Proceso.FechaInicio ||
                ahora > padron.Proceso.FechaFin)
            {
                return BadRequest("El proceso electoral no está activo.");
            }

            return Ok(new
            {
                PadronId = padron.Id
            });
        }

        private bool PadronExists(int id)
        {
            return _context.Padrones.Any(e => e.Id == id);
        }
    }
}
