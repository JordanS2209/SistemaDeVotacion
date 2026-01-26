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
    public class TiposIdentificacionesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public TiposIdentificacionesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/TiposIdentificaciones
        [HttpGet]
        public async Task<ActionResult<List<TipoIdentificacion>>> GetTiposIdentificaciones()
        {
            try
            {
                var tipos = await _context.TiposIdentificaciones.ToListAsync();
                return Ok(tipos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener tipos de identificación: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/TiposIdentificaciones/Codigo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoIdentificacion>> GetTipoIdentificacion(int id)
        {
            try
            {
                var tipo = await _context.TiposIdentificaciones
                    .Include(t => t.Usuarios)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tipo == null)
                {
                    return NotFound($"No se encontró el tipo de identificación con ID {id}.");
                }

                return Ok(tipo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetTipoIdentificacion: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/TiposIdentificaciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoIdentificacion(int id, TipoIdentificacion tipo)
        {
            if (id != tipo.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del objeto.");
            }

            _context.Entry(tipo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoIdentificacionExists(id))
                {
                    return NotFound("Tipo de identificación no encontrado.");
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

        // POST: api/TiposIdentificaciones
        [HttpPost]
        public async Task<ActionResult<TipoIdentificacion>> PostTipoIdentificacion(TipoIdentificacion tipo)
        {
            try
            {
                _context.TiposIdentificaciones.Add(tipo);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTipoIdentificacion), new { id = tipo.Id }, tipo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear tipo de identificación: {ex.Message}");
                return StatusCode(500, $"Error al guardar: {ex.Message}");
            }
        }

        // DELETE: api/TiposIdentificaciones/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TipoIdentificacion>> DeleteTipoIdentificacion(int id)
        {
            try
            {
                var tipo = await _context.TiposIdentificaciones.FindAsync(id);
                if (tipo == null)
                {
                    return NotFound("Tipo de identificación no encontrado.");
                }

                _context.TiposIdentificaciones.Remove(tipo);
                await _context.SaveChangesAsync();

                return Ok(tipo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool TipoIdentificacionExists(int id)
        {
            return _context.TiposIdentificaciones.Any(e => e.Id == id);
        }
    }
}