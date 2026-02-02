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
    public class OpcionesConsultasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public OpcionesConsultasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/OpcionesConsultas
        [HttpGet]
        public async Task<ActionResult<List<OpcionConsulta>>> GetOpcionConsultas()
        {
            try
            {
                var opciones = await _context.OpcionConsultas
                    .Include(o => o.Pregunta)
                    .ToListAsync();
                return Ok(opciones);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener opciones de consulta: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/OpcionesConsultas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OpcionConsulta>> GetOpcionConsulta(int id)
        {
            try
            {
                var opcion = await _context.OpcionConsultas
                    .Include(o => o.Pregunta)
                    .Include(o => o.VotosRecibidos)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (opcion == null)
                {
                    return NotFound($"No se encontró la opción de consulta con ID {id}.");
                }

                return Ok(opcion);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetOpcionConsulta: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/OpcionesConsultas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOpcionConsulta(int id, OpcionConsulta opcion)
        {
            if (id != opcion.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la opción.");
            }

            _context.Entry(opcion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OpcionConsultaExists(id))
                {
                    return NotFound("La opción de consulta no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la opción: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/OpcionesConsultas
        [HttpPost]
        public async Task<ActionResult<OpcionConsulta>> PostOpcionConsulta(OpcionConsulta opcion)
        {
            try
            {
                _context.OpcionConsultas.Add(opcion);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOpcionConsulta), new { id = opcion.Id }, opcion);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la opción de consulta: {ex.Message}");
                return StatusCode(500, $"Error al guardar la opción: {ex.Message}");
            }
        }

        // DELETE: api/OpcionesConsultas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<OpcionConsulta>> DeleteOpcionConsulta(int id)
        {
            try
            {
                var opcion = await _context.OpcionConsultas.FindAsync(id);
                if (opcion == null)
                {
                    return NotFound("Opción de consulta no encontrada.");
                }

                _context.OpcionConsultas.Remove(opcion);
                await _context.SaveChangesAsync();

                return Ok(opcion); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la opción: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool OpcionConsultaExists(int id)
        {
            return _context.OpcionConsultas.Any(e => e.Id == id);
        }
    }
}