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
    public class TipoProcesosController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public TipoProcesosController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/TipoProcesos
        [HttpGet]
        public async Task<ActionResult<List<TipoProceso>>> GetTipoProcesos()
        {
            try
            {
                var tipos = await _context.TipoProcesos
                    .AsNoTracking()
                    .OrderBy(tp => tp.NombreTipoProceso)
                    .ToListAsync();

                return Ok(tipos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener tipos de proceso: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }

        }

        // GET: api/TipoProcesos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoProceso>> GetTipoProceso(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("ID inválido.");

                var tipo = await _context.TipoProcesos
                    .AsNoTracking()
                    .Include(tp => tp.ProcesosAsociados)
                    .FirstOrDefaultAsync(tp => tp.Id == id);

                if (tipo == null)
                {
                    return NotFound($"No se encontró el tipo de proceso con ID {id}.");
                }

                return Ok(tipo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetTipoProceso: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }

        }

        // POST: api/TipoProcesos
        [HttpPost]
        public async Task<ActionResult<TipoProceso>> PostTipoProceso(TipoProceso nuevoTipo)
        {
            try
            {
                if (nuevoTipo == null)
                    return BadRequest("El cuerpo de la petición está vacío.");

                if (string.IsNullOrWhiteSpace(nuevoTipo.NombreTipoProceso))
                    return BadRequest("NombreTipoProceso es obligatorio.");

                // evitar duplicados por nombre 
                var existe = await _context.TipoProcesos
                    .AnyAsync(tp => tp.NombreTipoProceso.ToLower() == nuevoTipo.NombreTipoProceso.Trim().ToLower());

                if (existe)
                    return Conflict("Ya existe un Tipo de Proceso con el mismo nombre.");

                nuevoTipo.NombreTipoProceso = nuevoTipo.NombreTipoProceso.Trim();
                if (!string.IsNullOrWhiteSpace(nuevoTipo.Descripcion))
                    nuevoTipo.Descripcion = nuevoTipo.Descripcion.Trim();

                _context.TipoProcesos.Add(nuevoTipo);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTipoProceso), new { id = nuevoTipo.Id }, nuevoTipo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear tipo de proceso: {ex.Message}");
                return StatusCode(500, $"Error al guardar el tipo de proceso: {ex.Message}");
            }

        }

        // PUT: api/TipoProcesos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoProceso(int id, TipoProceso tipoProceso)
        {
            if (id != tipoProceso.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del tipo de proceso.");
            }

            if (tipoProceso == null)
                return BadRequest("El cuerpo de la petición está vacío.");

            if (string.IsNullOrWhiteSpace(tipoProceso.NombreTipoProceso))
                return BadRequest("NombreTipoProceso es obligatorio.");

            try
            {
                // Traemos el existente para no sobreescribir navegación/relaciones accidentalmente
                var existente = await _context.TipoProcesos.FirstOrDefaultAsync(tp => tp.Id == id);
                if (existente == null)
                    return NotFound("El tipo de proceso no existe.");

                // Opcional: evitar duplicado de nombre
                var nuevoNombre = tipoProceso.NombreTipoProceso.Trim();
                var existeOtro = await _context.TipoProcesos
                    .AnyAsync(tp => tp.Id != id && tp.NombreTipoProceso.ToLower() == nuevoNombre.ToLower());

                if (existeOtro)
                    return Conflict("Ya existe otro Tipo de Proceso con ese nombre.");

                existente.NombreTipoProceso = nuevoNombre;
                existente.Descripcion = string.IsNullOrWhiteSpace(tipoProceso.Descripcion)
                    ? null
                    : tipoProceso.Descripcion.Trim();

                await _context.SaveChangesAsync();
                return Ok("Tipo de proceso actualizado correctamente.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoProcesoExists(id))
                {
                    return NotFound("El tipo de proceso no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar tipo de proceso: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }

        }

        // DELETE: api/TipoProcesos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TipoProceso>> DeleteTipoProceso(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("ID inválido.");

                // incluir procesos asociados para validar FK antes de borrar
                var tipo = await _context.TipoProcesos
                    .Include(tp => tp.ProcesosAsociados)
                    .FirstOrDefaultAsync(tp => tp.Id == id);

                if (tipo == null)
                {
                    return NotFound("Tipo de proceso no encontrado.");
                }

                // Si tiene procesos asociados
                if (tipo.ProcesosAsociados != null && tipo.ProcesosAsociados.Any())
                {
                    return Conflict("No se puede eliminar: este Tipo de Proceso tiene procesos electorales asociados.");
                }

                _context.TipoProcesos.Remove(tipo);
                await _context.SaveChangesAsync();

                return Ok(tipo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar tipo de proceso: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }

        }

        private bool TipoProcesoExists(int id)
        {
            return _context.TipoProcesos.Any(tp => tp.Id == id);
        }
    }
}
