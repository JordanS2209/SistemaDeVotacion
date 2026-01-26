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
    public class RepresentantesJuntasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public RepresentantesJuntasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/RepresentantesJuntas
        [HttpGet]
        public async Task<ActionResult<List<RepresentanteJunta>>> GetRepresentantesJuntas()
        {
            try
            {
                // Incluimos información clave para el listado administrativo
                var representantes = await _context.RepresentantesJuntas
                    .Include(r => r.Usuario)
                    .Include(r => r.Junta)
                    .Include(r => r.Rol)
                    .Include(r => r.Proceso)
                    .ToListAsync();
                return Ok(representantes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener representantes de juntas: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/RepresentantesJuntas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RepresentanteJunta>> GetRepresentanteJunta(int id)
        {
            try
            {
                var representante = await _context.RepresentantesJuntas
                    .Include(r => r.Usuario)
                    .Include(r => r.Junta)
                    .Include(r => r.Rol)
                    .Include(r => r.Proceso)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (representante == null)
                {
                    return NotFound($"No se encontró el representante con ID {id}.");
                }

                return Ok(representante);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetRepresentanteJunta: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/RepresentantesJuntas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRepresentanteJunta(int id, RepresentanteJunta representante)
        {
            if (id != representante.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del representante.");
            }

            _context.Entry(representante).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RepresentanteJuntaExists(id))
                {
                    return NotFound("El representante de junta no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar representante: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/RepresentantesJuntas
        [HttpPost]
        public async Task<ActionResult<RepresentanteJunta>> PostRepresentanteJunta(RepresentanteJunta representante)
        {
            try
            {
                _context.RepresentantesJuntas.Add(representante);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetRepresentanteJunta), new { id = representante.Id }, representante);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear representante de junta: {ex.Message}");
                return StatusCode(500, $"Error al guardar: {ex.Message}");
            }
        }

        // DELETE: api/RepresentantesJuntas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RepresentanteJunta>> DeleteRepresentanteJunta(int id)
        {
            try
            {
                var representante = await _context.RepresentantesJuntas.FindAsync(id);
                if (representante == null)
                {
                    return NotFound("Representante de junta no encontrado.");
                }

                _context.RepresentantesJuntas.Remove(representante);
                await _context.SaveChangesAsync();

                return Ok(representante);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar representante: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool RepresentanteJuntaExists(int id)
        {
            return _context.RepresentantesJuntas.Any(e => e.Id == id);
        }
    }
}