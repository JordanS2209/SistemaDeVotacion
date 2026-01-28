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
    public class JuntasReceptorasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public JuntasReceptorasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/JuntasReceptoras
        [HttpGet]
        public async Task<ActionResult<List<JuntaReceptora>>> GetJuntasReceptoras()
        {
            try
            {
                // Incluimos Recintos y Genero para mostrar ubicación y tipo de junta en el listado
                var juntas = await _context.JuntasReceptoras
                    .Include(j => j.Recintos)
                    .Include(j => j.Genero)
                    .ToListAsync();
                return Ok(juntas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener Juntas Receptoras: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/JuntasReceptoras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JuntaReceptora>> GetJuntaReceptora(int id)
        {
            try
            {
                var junta = await _context.JuntasReceptoras
                    .Include(j => j.Genero)
                    .Include(j => j.Recintos)
                    .Include(j => j.VotantesAsignados)
                    .Include(j => j.Representantes)
                    .Include(j => j.VotosRecibidos)
                    .Include(j => j.ActasCierre)
                    .FirstOrDefaultAsync(j => j.Id == id);

                if (junta == null)
                {
                    return NotFound($"No se encontró la Junta Receptora con ID {id}.");
                }

                return Ok(junta);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetJuntaReceptora: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/JuntasReceptoras/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJuntaReceptora(int id, JuntaReceptora junta)
        {
            if (id != junta.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la junta.");
            }

            _context.Entry(junta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JuntaReceptoraExists(id))
                {
                    return NotFound("La Junta Receptora no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar Junta Receptora: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/JuntasReceptoras
        [HttpPost]
        public async Task<ActionResult<JuntaReceptora>> PostJuntaReceptora(JuntaReceptora junta)
        {
            try
            {
                _context.JuntasReceptoras.Add(junta);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetJuntaReceptora), new { id = junta.Id }, junta);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear Junta Receptora: {ex.Message}");
                return StatusCode(500, $"Error al guardar: {ex.Message}");
            }
        }

        // DELETE: api/JuntasReceptoras/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<JuntaReceptora>> DeleteJuntaReceptora(int id)
        {
            try
            {
                var junta = await _context.JuntasReceptoras.FindAsync(id);
                if (junta == null)
                {
                    return NotFound("Junta Receptora no encontrada.");
                }

                _context.JuntasReceptoras.Remove(junta);
                await _context.SaveChangesAsync();

                return Ok(junta);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar Junta Receptora: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool JuntaReceptoraExists(int id)
        {
            return _context.JuntasReceptoras.Any(e => e.Id == id);
        }
    }
}