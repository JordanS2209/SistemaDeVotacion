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
    public class ParroquiasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public ParroquiasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/Parroquias
        [HttpGet]
        public async Task<ActionResult<List<Parroquia>>> GetParroquias()
        {
            try
            {
                // Incluimos Recintos para que el MVC pueda mostrar el conteo por parroquia
                var parroquias = await _context.Parroquias
                    .Include(p => p.Recintos)
                    .Include(p => p.Ciudad)
                    .ToListAsync();
                return Ok(parroquias);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener parroquias: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/Parroquias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Parroquia>> GetParroquia(int id)
        {
            try
            {
                var parroquia = await _context.Parroquias
                    .Include(p => p.Ciudad)
                    .Include(p => p.Recintos)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (parroquia == null)
                {
                    return NotFound($"No se encontró la parroquia con ID {id}.");
                }

                return Ok(parroquia);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetParroquia: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/Parroquias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParroquia(int id, Parroquia parroquia)
        {
            if (id != parroquia.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la parroquia.");
            }

            _context.Entry(parroquia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParroquiaExists(id))
                {
                    return NotFound("La parroquia no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/Parroquias
        [HttpPost]
        public async Task<ActionResult<Parroquia>> PostParroquia(Parroquia parroquia)
        {
            try
            {
                _context.Parroquias.Add(parroquia);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetParroquia), new { id = parroquia.Id }, parroquia);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear parroquia: {ex.Message}");
                return StatusCode(500, $"Error al guardar la parroquia: {ex.Message}");
            }
        }

        // DELETE: api/Parroquias/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Parroquia>> DeleteParroquia(int id)
        {
            try
            {
                var parroquia = await _context.Parroquias.FindAsync(id);
                if (parroquia == null)
                {
                    return NotFound("Parroquia no encontrada.");
                }

                _context.Parroquias.Remove(parroquia);
                await _context.SaveChangesAsync();

                return Ok(parroquia);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar la parroquia: {ex.Message}");
            }
        }

        private bool ParroquiaExists(int id)
        {
            return _context.Parroquias.Any(e => e.Id == id);
        }
    }
}