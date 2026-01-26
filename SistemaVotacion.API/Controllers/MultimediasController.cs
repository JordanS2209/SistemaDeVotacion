using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotacion.Modelos;

namespace SistemaVotacion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MultimediasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public MultimediasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/Multimedias
        [HttpGet]
        public async Task<ActionResult<List<Multimedia>>> GetMultimedias()
        {
            try
            {
                var multimedias = await _context.Multimedias
                    .Include(m => m.Candidato)
                    .Include(m => m.Lista)
                    .ToListAsync();
                return Ok(multimedias);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener recursos multimedia: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/Multimedias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Multimedia>> GetMultimedia(int id)
        {
            try
            {
                var multimedia = await _context.Multimedias
                    .Include(m => m.Candidato)
                    .Include(m => m.Lista)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (multimedia == null)
                {
                    return NotFound($"No se encontró el recurso multimedia con ID {id}.");
                }

                return Ok(multimedia);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetMultimedia: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/Multimedias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMultimedia(int id, Multimedia multimedia)
        {
            if (id != multimedia.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del recurso.");
            }

            _context.Entry(multimedia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MultimediaExists(id))
                {
                    return NotFound("El recurso multimedia no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar multimedia: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/Multimedias
        [HttpPost]
        public async Task<ActionResult<Multimedia>> PostMultimedia(Multimedia multimedia)
        {
            try
            {
                _context.Multimedias.Add(multimedia);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetMultimedia), new { id = multimedia.Id }, multimedia);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear multimedia: {ex.Message}");
                return StatusCode(500, $"Error al guardar el recurso: {ex.Message}");
            }
        }

        // DELETE: api/Multimedias/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Multimedia>> DeleteMultimedia(int id)
        {
            try
            {
                var multimedia = await _context.Multimedias.FindAsync(id);
                if (multimedia == null)
                {
                    return NotFound("Recurso multimedia no encontrado.");
                }

                _context.Multimedias.Remove(multimedia);
                await _context.SaveChangesAsync();

                return Ok(multimedia); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar multimedia: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool MultimediaExists(int id)
        {
            return _context.Multimedias.Any(e => e.Id == id);
        }
    }
}