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
    public class RecintosElectoralesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public RecintosElectoralesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/RecintosElectorales
        [HttpGet]
        public async Task<ActionResult<List<RecintoElectoral>>> GetRecintosElectorales()
        {
            try
            {
                
                var recintos = await _context.RecintosElectorales
                    .Include(r => r.Parroquia)
                    .ToListAsync();
                return Ok(recintos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener recintos electorales: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/RecintosElectorales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecintoElectoral>> GetRecintoElectoral(int id)
        {
            try
            {
                var recinto = await _context.RecintosElectorales
                    .Include(r => r.Parroquia)
                    .Include(r => r.JuntasReceptoras)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (recinto == null)
                {
                    return NotFound($"No se encontró el recinto electoral con ID {id}.");
                }

                return Ok(recinto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetRecintoElectoral: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/RecintosElectorales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecintoElectoral(int id, RecintoElectoral recinto)
        {
            if (id != recinto.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del recinto.");
            }

            _context.Entry(recinto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecintoElectoralExists(id))
                {
                    return NotFound("El recinto electoral no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar recinto: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/RecintosElectorales
        [HttpPost]
        public async Task<ActionResult<RecintoElectoral>> PostRecintoElectoral(RecintoElectoral recinto)
        {
            try
            {
                _context.RecintosElectorales.Add(recinto);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetRecintoElectoral), new { id = recinto.Id }, recinto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear recinto electoral: {ex.Message}");
                return StatusCode(500, $"Error al guardar el recinto: {ex.Message}");
            }
        }

        // DELETE: api/RecintosElectorales/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RecintoElectoral>> DeleteRecintoElectoral(int id)
        {
            try
            {
                var recinto = await _context.RecintosElectorales.FindAsync(id);
                if (recinto == null)
                {
                    return NotFound("Recinto electoral no encontrado.");
                }

                _context.RecintosElectorales.Remove(recinto);
                await _context.SaveChangesAsync();

                return Ok(recinto); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar recinto: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool RecintoElectoralExists(int id)
        {
            return _context.RecintosElectorales.Any(e => e.Id == id);
        }
    }
}