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
    public class TipoVotosController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public TipoVotosController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/TipoVotos
        [HttpGet]
        public async Task<ActionResult<List<TipoVoto>>> GetTipoVotos()
        {
            try
            {
                var tipos = await _context.TipoVotos.ToListAsync();
                return Ok(tipos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener tipos de votos: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/TipoVotos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoVoto>> GetTipoVoto(int id)
        {
            try
            {
                var tipo = await _context.TipoVotos
                    .Include(t => t.VotosAsociados)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tipo == null)
                {
                    return NotFound($"No se encontró el tipo de voto con ID {id}.");
                }

                return Ok(tipo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetTipoVoto: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/TipoVotos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoVoto(int id, TipoVoto tipo)
        {
            if (id != tipo.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del tipo de voto.");
            }

            _context.Entry(tipo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoVotoExists(id))
                {
                    return NotFound("El tipo de voto no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar tipo de voto: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/TipoVotos
        [HttpPost]
        public async Task<ActionResult<TipoVoto>> PostTipoVoto(TipoVoto tipo)
        {
            try
            {
                _context.TipoVotos.Add(tipo);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTipoVoto), new { id = tipo.Id }, tipo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear tipo de voto: {ex.Message}");
                return StatusCode(500, $"Error al guardar el tipo de voto: {ex.Message}");
            }
        }

        // DELETE: api/TipoVotos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TipoVoto>> DeleteTipoVoto(int id)
        {
            try
            {
                var tipo = await _context.TipoVotos.FindAsync(id);
                if (tipo == null)
                {
                    return NotFound("Tipo de voto no encontrado.");
                }

                _context.TipoVotos.Remove(tipo);
                await _context.SaveChangesAsync();

                return Ok(tipo); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar tipo de voto: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool TipoVotoExists(int id)
        {
            return _context.TipoVotos.Any(e => e.Id == id);
        }
    }
}