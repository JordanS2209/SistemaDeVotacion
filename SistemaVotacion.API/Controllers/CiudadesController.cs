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
    public class CiudadesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public CiudadesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/Ciudades
        [HttpGet]
        public async Task<ActionResult<List<Ciudad>>> GetCiudades()
        {
            try
            {
                // Incluimos Parroquias para poder contarlas en el listado del MVC
                var ciudades = await _context.Ciudades
                    .Include(c => c.Provincia)
                    .Include(c => c.Parroquias)
                    .ToListAsync();
                return Ok(ciudades);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener ciudades: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/Ciudades/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ciudad>> GetCiudad(int id)
        {
            try
            {
                var ciudad = await _context.Ciudades
                    .Include(c => c.Provincia)
                    .Include(c => c.Parroquias)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (ciudad == null)
                {
                    return NotFound($"No se encontró la ciudad con ID {id}.");
                }

                return Ok(ciudad);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetCiudad: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/Ciudades/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCiudad(int id, Ciudad ciudad)
        {
            if (id != ciudad.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la ciudad.");
            }

            _context.Entry(ciudad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); // 204 No Content
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CiudadExists(id))
                {
                    return NotFound("La ciudad no existe.");
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

        // POST: api/Ciudades
        [HttpPost]
        public async Task<ActionResult<Ciudad>> PostCiudad(Ciudad ciudad)
        {
            try
            {
                _context.Ciudades.Add(ciudad);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCiudad), new { id = ciudad.Id }, ciudad);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear ciudad: {ex.Message}");
                return StatusCode(500, $"Error al guardar la ciudad: {ex.Message}");
            }
        }

        // DELETE: api/Ciudades/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Ciudad>> DeleteCiudad(int id)
        {
            try
            {
                var ciudad = await _context.Ciudades.FindAsync(id);
                if (ciudad == null)
                {
                    return NotFound("Ciudad no encontrada.");
                }

                _context.Ciudades.Remove(ciudad);
                await _context.SaveChangesAsync();

                return Ok(ciudad);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar la ciudad: {ex.Message}");
            }
        }

        private bool CiudadExists(int id)
        {
            return _context.Ciudades.Any(e => e.Id == id);
        }
    }
}