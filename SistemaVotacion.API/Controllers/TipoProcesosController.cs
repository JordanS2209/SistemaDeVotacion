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
    public class TipoProcesosController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public TipoProcesosController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/TipoProcesos
        [HttpGet]
        public async Task<ActionResult<List<TipoProceso>>> GetTipoProcesos()
        {
            try
            {
                var tipos = await _context.TipoProcesos.ToListAsync();
                return Ok(tipos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener tipos de procesos: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/TipoProcesos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoProceso>> GetTipoProceso(int id)
        {
            try
            {
                var tipo = await _context.TipoProcesos
                    .Include(t => t.ProcesosAsociados)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tipo == null)
                {
                    return NotFound($"No se encontró el tipo de proceso con ID {id}.");
                }

                return Ok(tipo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetTipoProceso: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/TipoProcesos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoProceso(int id, TipoProceso tipo)
        {
            if (id != tipo.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del tipo de proceso.");
            }

            _context.Entry(tipo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoProcesoExists(id))
                {
                    return NotFound("El tipo de proceso no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar tipo de proceso: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/TipoProcesos
        [HttpPost]
        public async Task<ActionResult<TipoProceso>> PostTipoProceso(TipoProceso tipo)
        {
            try
            {
                _context.TipoProcesos.Add(tipo);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTipoProceso), new { id = tipo.Id }, tipo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear tipo de proceso: {ex.Message}");
                return StatusCode(500, $"Error al guardar: {ex.Message}");
            }
        }

        // DELETE: api/TipoProcesos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TipoProceso>> DeleteTipoProceso(int id)
        {
            try
            {
                var tipo = await _context.TipoProcesos.FindAsync(id);
                if (tipo == null)
                {
                    return NotFound("Tipo de proceso no encontrado.");
                }

                _context.TipoProcesos.Remove(tipo);
                await _context.SaveChangesAsync();

                return Ok(tipo); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar tipo de proceso: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool TipoProcesoExists(int id)
        {
            return _context.TipoProcesos.Any(e => e.Id == id);
        }
    }
}