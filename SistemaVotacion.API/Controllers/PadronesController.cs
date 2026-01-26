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
                // Incluimos Proceso y Votante para mostrar quién vota y en qué elección
                var padrones = await _context.Padrones
                    .Include(p => p.Proceso)
                    .Include(p => p.Votante)
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
                    .Include(p => p.Proceso)
                    .Include(p => p.Votante)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (padron == null)
                {
                    return NotFound($"Registro de padrón con ID {id} no encontrado.");
                }

                return Ok(padron);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetPadron: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/Padrones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPadron(int id, Padron padron)
        {
            if (id != padron.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del registro de padrón.");
            }

            _context.Entry(padron).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PadronExists(id))
                {
                    return NotFound("El registro de padrón no existe.");
                }
                else
                {
                    throw;
                }
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
                {
                    return NotFound("Registro de padrón no encontrado.");
                }

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

        private bool PadronExists(int id)
        {
            return _context.Padrones.Any(e => e.Id == id);
        }
    }
}