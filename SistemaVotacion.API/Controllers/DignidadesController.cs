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
    public class DignidadesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public DignidadesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/Dignidades
        [HttpGet]
        public async Task<ActionResult<List<Dignidad>>> GetDignidades()
        {
            try
            {
                var dignidades = await _context.Dignidades.ToListAsync();
                return Ok(dignidades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/Dignidades/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Dignidad>> GetDignidad(int id)
        {
            try
            {
                var dignidad = await _context.Dignidades
                    .Include(d => d.Candidatos)
                    .Include(d => d.VotosRecibidos)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (dignidad == null)
                {
                    return NotFound($"Dignidad con ID {id} no encontrada.");
                }

                return Ok(dignidad); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
        // GET: api/Dignidades/simple
        [HttpGet("simple")]
        public async Task<ActionResult<IEnumerable<object>>> GetDignidadesSimple()
        {
            var dignidades = await _context.Dignidades
                .Select(d => new { d.Id, d.NombreDignidad })
                .ToListAsync();

            return Ok(dignidades);
        }


        // PUT: api/Dignidades/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDignidad(int id, Dignidad dignidad)
        {
            if (id != dignidad.Id)
            {
                return BadRequest("ID de Dignidad no coincide.");
            }

            _context.Entry(dignidad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DignidadExists(id))
                {
                    return NotFound("Dignidad no encontrada.");
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

        // POST: api/Dignidades
        [HttpPost]
        public async Task<ActionResult<Dignidad>> PostDignidad(Dignidad dignidad)
        {
            try
            {
                _context.Dignidades.Add(dignidad);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetDignidad), new { id = dignidad.Id }, dignidad);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar: {ex.Message}");
            }
        }

        // DELETE: api/Dignidades/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Dignidad>> DeleteDignidad(int id)
        {
            try
            {
                var dignidad = await _context.Dignidades.FindAsync(id);
                if (dignidad == null)
                {
                    return NotFound("Dignidad no encontrada.");
                }

                _context.Dignidades.Remove(dignidad);
                await _context.SaveChangesAsync();

                return Ok(dignidad); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool DignidadExists(int id)
        {
            return _context.Dignidades.Any(e => e.Id == id);
        }
    }
}