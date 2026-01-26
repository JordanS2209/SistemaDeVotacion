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
    public class PreguntasConsultasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public PreguntasConsultasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/PreguntasConsultas
        [HttpGet]
        public async Task<ActionResult<List<PreguntaConsulta>>> GetPreguntasConsultas()
        {
            try
            {
                // Incluimos el Proceso Electoral para saber a qué consulta pertenece cada pregunta
                var preguntas = await _context.PreguntasConsultas
                    .Include(p => p.ProcesosElectorales)
                    .ToListAsync();
                return Ok(preguntas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener preguntas de consulta: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/PreguntasConsultas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PreguntaConsulta>> GetPreguntaConsulta(int id)
        {
            try
            {
                var pregunta = await _context.PreguntasConsultas
                    .Include(p => p.ProcesosElectorales)
                    .Include(p => p.OpcionesConsulta) 
                    .Include(p => p.VotosRecibidos)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pregunta == null)
                {
                    return NotFound($"No se encontró la pregunta de consulta con ID {id}.");
                }

                return Ok(pregunta);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetPreguntaConsulta: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/PreguntasConsultas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPreguntaConsulta(int id, PreguntaConsulta pregunta)
        {
            if (id != pregunta.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la pregunta.");
            }

            _context.Entry(pregunta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PreguntaConsultaExists(id))
                {
                    return NotFound("La pregunta de consulta no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la pregunta: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/PreguntasConsultas
        [HttpPost]
        public async Task<ActionResult<PreguntaConsulta>> PostPreguntaConsulta(PreguntaConsulta pregunta)
        {
            try
            {
                _context.PreguntasConsultas.Add(pregunta);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPreguntaConsulta), new { id = pregunta.Id }, pregunta);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la pregunta de consulta: {ex.Message}");
                return StatusCode(500, $"Error al guardar: {ex.Message}");
            }
        }

        // DELETE: api/PreguntasConsultas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PreguntaConsulta>> DeletePreguntaConsulta(int id)
        {
            try
            {
                var pregunta = await _context.PreguntasConsultas.FindAsync(id);
                if (pregunta == null)
                {
                    return NotFound("Pregunta de consulta no encontrada.");
                }

                _context.PreguntasConsultas.Remove(pregunta);
                await _context.SaveChangesAsync();

                return Ok(pregunta);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la pregunta: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool PreguntaConsultaExists(int id)
        {
            return _context.PreguntasConsultas.Any(e => e.Id == id);
        }
    }
}