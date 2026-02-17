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
    public class VotantesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public VotantesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/Votantes
        [HttpGet]
        public async Task<ActionResult<List<Votante>>> GetVotantes()
        {
            try
            {
                var votantes = await _context.Votantes
                    .Include(v => v.Usuario)
                    .Include(v => v.Junta)
                    .ToListAsync();

                return Ok(votantes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener votantes: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/Votantes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Votante>> GetVotante(int id)
        {
            try
            {
                var votante = await _context.Votantes
                    .Include(v => v.Usuario)
                    .Include(v => v.Junta)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (votante == null)
                    return NotFound($"No se encontró el votante con ID {id}.");

                return Ok(votante);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetVotante: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/Votantes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVotante(int id, Votante votante)
        {
            if (id != votante.Id)
                return BadRequest("El ID de la URL no coincide con el ID del votante.");

            _context.Entry(votante).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VotanteExists(id))
                    return NotFound("El votante no existe.");
                else
                    throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar votante: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/Votantes
        [HttpPost]
        public async Task<ActionResult<Votante>> PostVotante(Votante votante)
        {

            try
            {
                _context.Votantes.Add(votante);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetVotante), new { id = votante.Id }, votante);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear votante: {ex.Message}");
                return StatusCode(500, $"Error al guardar el votante: {ex.Message}");
            }
        }

        // DELETE: api/Votantes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Votante>> DeleteVotante(int id)
        {
            try
            {
                var votante = await _context.Votantes.FindAsync(id);
                if (votante == null)
                    return NotFound("Votante no encontrado.");

                _context.Votantes.Remove(votante);
                await _context.SaveChangesAsync();

                return Ok(votante);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar votante: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool VotanteExists(int id)
        {
            return _context.Votantes.Any(e => e.Id == id);
        }
    }
}