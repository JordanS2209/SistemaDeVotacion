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

        // GET: api/Padrones
        [HttpGet]
        public async Task<ActionResult<List<Padron>>> GetPadrones()
        {
            try
            {
                var padrones = await _context.Padrones
                    .Include(p => p.Votante)
                        .ThenInclude(v => v.Usuario)  // nombre y apellido
                    .Include(p => p.Votante)
                        .ThenInclude(v => v.Junta)    // número de junta
                    .Include(p => p.Proceso)
                    .ToListAsync();

                return Ok(padrones);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el padrón electoral: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/Padrones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Padron>> GetPadron(int id)
        {
            try
            {
                var padron = await _context.Padrones
                    .Include(p => p.Votante)
                        .ThenInclude(v => v.Usuario)
                    .Include(p => p.Votante)
                        .ThenInclude(v => v.Junta)
                    .Include(p => p.Proceso)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (padron == null)
                    return NotFound($"Registro de padrón con ID {id} no encontrado.");

                return Ok(padron);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetPadron: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
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
        public async Task<IActionResult> PutPadron(int id, Padron padron)
        {
            if (id != padron.Id)
                return BadRequest("El ID de la URL no coincide con el ID del registro de padrón.");

            _context.Entry(padron).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PadronExists(id))
                    return NotFound("El registro de padrón no existe.");
                else
                    throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar padrón: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/Padrones
        [HttpPost]
        public async Task<ActionResult<Padron>> PostPadron(Padron padron)
        {
            try
            {
                _context.Padrones.Add(padron);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPadron), new { id = padron.Id }, padron);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear registro en el padrón: {ex.Message}");
                return StatusCode(500, $"Error al guardar: {ex.Message}");
            }
        }

        // DELETE: api/Padrones/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Padron>> DeletePadron(int id)
        {
            try
            {
                var padron = await _context.Padrones.FindAsync(id);
                if (padron == null)
                    return NotFound("Registro de padrón no encontrado.");

                _context.Padrones.Remove(padron);
                await _context.SaveChangesAsync();
                return Ok(padron);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar del padrón: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
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
                PadronId = padron.Id,
                IdProceso = padron.IdProceso
            });

        }

        private bool PadronExists(int id)
        {
            return _context.Padrones.Any(e => e.Id == id);
        }
    }
}