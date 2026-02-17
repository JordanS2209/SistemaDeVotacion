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
                var procesos = await _context.ProcesosElectorales
                    .AsNoTracking()
                    .Include(p => p.TipoProceso)
                    .OrderByDescending(p => p.FechaInicio)
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
                if (id <= 0)
                    return BadRequest("ID inválido.");

                var proceso = await _context.ProcesosElectorales
                    .AsNoTracking()
                    .Include(p => p.TipoProceso)
                    .Include(p => p.PadronElectoral)
                    .Include(p => p.ListasParticipantes)
                    .Include(p => p.PreguntasConsulta)
                    .Include(p => p.VotoDetallados)
                    .Include(p => p.RepresentantesMesas)
                    .Include(p => p.ActasGeneradas)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (proceso == null)
                    return NotFound($"No se encontró el proceso electoral con ID {id}.");

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
            // OJO: Solape significa que dos intervalos de tiempo se cruzan o se superponen parcialmente o totalmente.
            try
            {
                if (proceso == null)
                    return BadRequest("El cuerpo de la petición está vacío.");

                if (id != proceso.Id)
                    return BadRequest("El ID no coincide.");

                if (string.IsNullOrWhiteSpace(proceso.NombreProceso))
                    return BadRequest("NombreProceso es obligatorio.");

                if (proceso.IdTipoProceso <= 0)
                    return BadRequest("IdTipoProceso es obligatorio.");

                // En edición normal siempre exigimos rango válido (NO permitimos el truco de pausado en input)
                if (proceso.FechaInicio == default || proceso.FechaFin == default)
                    return BadRequest("FechaInicio y FechaFin son obligatorias.");

                if (proceso.FechaInicio >= proceso.FechaFin)
                    return BadRequest("FechaInicio debe ser menor que FechaFin.");

                var procesoExistente = await _context.ProcesosElectorales
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (procesoExistente == null)
                    return NotFound("Proceso electoral no encontrado.");

                var ahora = DateTime.Now;

                // Estado "PAUSADO" = FechaInicio > FechaFin (truco interno)
                bool estaPausado = procesoExistente.FechaInicio > procesoExistente.FechaFin;

                //Regla: NO editar si está activo (solo si NO está pausado)
                bool estaActivo = !estaPausado && ahora >= procesoExistente.FechaInicio && ahora <= procesoExistente.FechaFin;
                if (estaActivo)
                    return BadRequest("No se puede modificar un proceso activo.");

                //Regla: NO editar si está vencido naturalmente (fecha fin ya pasó)
                if (ahora > procesoExistente.FechaFin)
                    return BadRequest("No se puede modificar un proceso cerrado.");

                // No permitir cambiar el tipo de proceso
                if (procesoExistente.IdTipoProceso != proceso.IdTipoProceso)
                    return BadRequest("No se puede cambiar el tipo de proceso.");

                // Validar FK
                var existeTipo = await _context.TipoProcesos.AnyAsync(tp => tp.Id == proceso.IdTipoProceso);
                if (!existeTipo)
                    return BadRequest("IdTipoProceso no existe.");

                //  se permite solape (no validamos intersección)

                // Actualizar campos permitidos
                procesoExistente.NombreProceso = proceso.NombreProceso.Trim();
                procesoExistente.FechaInicio = proceso.FechaInicio;
                procesoExistente.FechaFin = proceso.FechaFin;

                await _context.SaveChangesAsync();

                return Ok("Proceso actualizado correctamente.");
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
                if (proceso == null)
                    return BadRequest("Datos inválidos.");

                if (string.IsNullOrWhiteSpace(proceso.NombreProceso))
                    return BadRequest("NombreProceso es obligatorio.");

                if (proceso.IdTipoProceso <= 0)
                    return BadRequest("IdTipoProceso es obligatorio.");

                if (proceso.FechaInicio == default || proceso.FechaFin == default)
                    return BadRequest("FechaInicio y FechaFin son obligatorias.");

                if (proceso.FechaInicio >= proceso.FechaFin)
                    return BadRequest("La fecha de inicio debe ser menor a la fecha de fin.");

                // Validar FK (integridad referencial)
                var existeTipo = await _context.TipoProcesos
                    .AsNoTracking()
                    .AnyAsync(tp => tp.Id == proceso.IdTipoProceso);

                if (!existeTipo)
                    return BadRequest("IdTipoProceso no existe.");

                // Limpieza simple
                proceso.NombreProceso = proceso.NombreProceso.Trim();

                // NO validamos solape 
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


        // GET: api/ProcesosElectorales/activo
        [HttpGet("activo")]
        public async Task<ActionResult<ProcesoElectoral>> GetProcesoElectoralActivo()
        {
            try
            {
                var ahora = DateTime.Now;

                // 1️⃣ Obtener procesos activos por fecha
                var procesosActivos = await _context.ProcesosElectorales
                    .Include(p => p.TipoProceso)
                    .Where(p =>
                        ahora >= p.FechaInicio &&
                        ahora <= p.FechaFin
                    )
                    .ToListAsync();

                if (!procesosActivos.Any())
                {
                    return NotFound("No existe ningún proceso electoral activo.");
                }

                // 2️⃣ PRIORIDAD 1: ELECCIONES GENERALES
                var eleccionesGenerales = procesosActivos
                    .FirstOrDefault(p => p.IdTipoProceso == 1);

                if (eleccionesGenerales != null)
                {
                    return Ok(eleccionesGenerales);
                }

                // 3️⃣ PRIORIDAD 2: ELECCIONES SECCIONALES
                var eleccionesSeccionales = procesosActivos
                    .FirstOrDefault(p => p.IdTipoProceso == 3);

                if (eleccionesSeccionales != null)
                {
                    return Ok(eleccionesSeccionales);
                }

                // 4️⃣ PRIORIDAD 3: CONSULTA POPULAR
                var consultaPopular = procesosActivos
                    .FirstOrDefault(p => p.IdTipoProceso == 2);

                if (consultaPopular != null)
                {
                    return Ok(consultaPopular);
                }

                // 5️⃣ Fallback (seguridad)
                return Ok(procesosActivos.First());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener proceso activo: {ex.Message}");
            }
        }

        private bool ProcesoElectoralExists(int id)
        {
            return _context.ProcesosElectorales.Any(e => e.Id == id);
        }

        [HttpPut("Activar/{id}")]
        public async Task<IActionResult> ActivarProceso(int id)
        {
            try
            {
                var proceso = await _context.ProcesosElectorales
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (proceso == null)
                    return NotFound("Proceso electoral no encontrado.");

                var ahora = DateTime.Now;

                // Regla principal: no se puede activar si ya pasó la fecha fin programada
                if (ahora > proceso.FechaFin)
                    return BadRequest("No se puede activar: la fecha fin del proceso ya pasó.");

                // Si ya está activo, no repetir
                if (ahora >= proceso.FechaInicio && ahora <= proceso.FechaFin)
                    return BadRequest("El proceso ya está activo.");

                // Reactivar / Activar:
                proceso.FechaInicio = ahora;

                await _context.SaveChangesAsync();

                return Ok("Proceso activado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }

        [HttpPut("Cerrar/{id}")]
        public async Task<IActionResult> CerrarProceso(int id)
        {
            try
            {
                var proceso = await _context.ProcesosElectorales.FindAsync(id);

                if (proceso == null)
                    return NotFound("Proceso electoral no encontrado.");

                var ahora = DateTime.Now;

                // No pausar si ya venció naturalmente
                if (ahora > proceso.FechaFin)
                    return BadRequest("No se puede desactivar: la fecha fin del proceso ya pasó.");

                // Solo permitir pausar si está activo (recomendado)
                var estaActivo = ahora >= proceso.FechaInicio && ahora <= proceso.FechaFin;
                if (!estaActivo)
                    return BadRequest("Solo se puede desactivar (pausar) un proceso ACTIVO.");

                // Pausar SIN tocar FechaFin:
                // Truco: FechaInicio > FechaFin indica Pausado
                proceso.FechaInicio = proceso.FechaFin.AddSeconds(1);

                await _context.SaveChangesAsync();

                return Ok("Proceso desactivado (pausado) correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }



    }
}