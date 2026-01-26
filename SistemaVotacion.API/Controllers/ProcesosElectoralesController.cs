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
    public class ProcesosElectoralesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public ProcesosElectoralesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/ProcesosElectorales
        [HttpGet]
        public async Task<ActionResult<List<ProcesoElectoral>>> GetProcesosElectorales()
        {
            try
            {
                // Incluimos el tipo de proceso para el listado administrativo
                var procesos = await _context.ProcesosElectorales
                    .Include(p => p.TipoProceso)
                    .ToListAsync();
                return Ok(procesos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener procesos electorales: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/ProcesosElectorales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProcesoElectoral>> GetProcesoElectoral(int id)
        {
            try
            {
                var proceso = await _context.ProcesosElectorales
                    .Include(p => p.TipoProceso)
                    .Include(p => p.PadronElectoral)
                    .Include(p => p.ListasParticipantes)
                    .Include(p => p.DignidadesAElegir)
                    .Include(p => p.PreguntasConsulta)
                    .Include(p => p.VotoDetallados)
                    .Include(p => p.RepresentantesMesas)
                    .Include(p => p.ActasGeneradas)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (proceso == null)
                {
                    return NotFound($"No se encontró el proceso electoral con ID {id}.");
                }

                return Ok(proceso);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetProcesoElectoral: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/ProcesosElectorales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProcesoElectoral(int id, ProcesoElectoral proceso)
        {
            if (id != proceso.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del proceso electoral.");
            }

            _context.Entry(proceso).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProcesoElectoralExists(id))
                {
                    return NotFound("El proceso electoral no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar proceso electoral: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/ProcesosElectorales
        [HttpPost]
        public async Task<ActionResult<ProcesoElectoral>> PostProcesoElectoral(ProcesoElectoral proceso)
        {
            try
            {
                _context.ProcesosElectorales.Add(proceso);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProcesoElectoral), new { id = proceso.Id }, proceso);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear proceso electoral: {ex.Message}");
                return StatusCode(500, $"Error al guardar el proceso electoral: {ex.Message}");
            }
        }

        // DELETE: api/ProcesosElectorales/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProcesoElectoral>> DeleteProcesoElectoral(int id)
        {
            try
            {
                var proceso = await _context.ProcesosElectorales.FindAsync(id);
                if (proceso == null)
                {
                    return NotFound("Proceso electoral no encontrado.");
                }

                _context.ProcesosElectorales.Remove(proceso);
                await _context.SaveChangesAsync();

                return Ok(proceso);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar proceso electoral: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool ProcesoElectoralExists(int id)
        {
            return _context.ProcesosElectorales.Any(e => e.Id == id);
        }
    }
}