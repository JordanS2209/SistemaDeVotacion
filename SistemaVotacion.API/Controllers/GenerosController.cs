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
    public class GenerosController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public GenerosController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/Generos
        [HttpGet]
        public async Task<ActionResult<List<Genero>>> GetGeneros()
        {
            try
            {
                var generos = await _context.Generos.ToListAsync();
                return Ok(generos); // Retorna la lista directamente
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener géneros: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/Generos/Codigo/5
        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<Genero>> GetGenero(int id)
        {
            try
            {
                var genero = await _context.Generos
                    .Include(g => g.Usuarios)
                    .Include(g => g.Juntas)
                    .FirstOrDefaultAsync(g => g.IdGenero == id);

                if (genero == null)
                {
                    return NotFound($"No se encontró el género con ID {id}.");
                }

                return Ok(genero);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetGenero: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/Generos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGenero(int id, Genero genero)
        {
            if (id != genero.IdGenero)
            {
                return BadRequest("El ID de la URL no coincide con el ID del objeto.");
            }

            _context.Entry(genero).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); // 204 No Content: Éxito sin datos de retorno
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GeneroExists(id))
                {
                    return NotFound("Género no encontrado.");
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

        // POST: api/Generos
        [HttpPost]
        public async Task<ActionResult<Genero>> PostGenero(Genero genero)
        {
            try
            {
                _context.Generos.Add(genero);
                await _context.SaveChangesAsync();

                // Retorna 201 Created y la ubicación para consultar el nuevo recurso
                return CreatedAtAction(nameof(GetGenero), new { id = genero.IdGenero }, genero);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear género: {ex.Message}");
                return StatusCode(500, $"Error al guardar: {ex.Message}");
            }
        }

        // DELETE: api/Generos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Genero>> DeleteGenero(int id)
        {
            try
            {
                var genero = await _context.Generos.FindAsync(id);
                if (genero == null)
                {
                    return NotFound("Género no encontrado.");
                }

                _context.Generos.Remove(genero);
                await _context.SaveChangesAsync();

                return Ok(genero); // Retornamos el objeto que fue eliminado
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool GeneroExists(int id)
        {
            return _context.Generos.Any(e => e.IdGenero == id);
        }
    }
}