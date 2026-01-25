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
    public class ProvinciasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public ProvinciasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/Provincias
        [HttpGet]
        public async Task<ActionResult<List<Provincia>>> GetProvincias()
        {
            try
            {
                // Incluimos Ciudades para que el MVC pueda contar cuántas tiene cada provincia
                var provincias = await _context.Provincias
                    .Include(p => p.Ciudades)
                    .ToListAsync();
                return Ok(provincias);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener provincias: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/Provincias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Provincia>> GetProvincia(int id)
        {
            try
            {
                var provincia = await _context.Provincias
                    .Include(p => p.Ciudades)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (provincia == null)
                {
                    return NotFound($"No se encontró la provincia con ID {id}.");
                }

                return Ok(provincia);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetProvincia: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/Provincias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProvincia(int id, Provincia provincia)
        {
            if (id != provincia.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la provincia.");
            }

            _context.Entry(provincia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); // 204 No Content
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProvinciaExists(id))
                {
                    return NotFound("La provincia no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar la provincia: {ex.Message}");
            }
        }

        // POST: api/Provincias
        [HttpPost]
        public async Task<ActionResult<Provincia>> PostProvincia(Provincia provincia)
        {
            try
            {
                _context.Provincias.Add(provincia);
                await _context.SaveChangesAsync();

                // Retorna 201 Created con la ruta para obtener el recurso
                return CreatedAtAction(nameof(GetProvincia), new { id = provincia.Id }, provincia);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear provincia: {ex.Message}");
                return StatusCode(500, $"Error al guardar la provincia: {ex.Message}");
            }
        }

        // DELETE: api/Provincias/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Provincia>> DeleteProvincia(int id)
        {
            try
            {
                var provincia = await _context.Provincias.FindAsync(id);
                if (provincia == null)
                {
                    return NotFound("Provincia no encontrada.");
                }

                _context.Provincias.Remove(provincia);
                await _context.SaveChangesAsync();

                return Ok(provincia); // Retorna el objeto eliminado
            }
            catch (Exception ex)
            {
                // Este error suele suceder si hay ciudades asociadas (Error de FK)
                return StatusCode(500, $"Error al eliminar la provincia: {ex.Message}");
            }
        }

        private bool ProvinciaExists(int id)
        {
            return _context.Provincias.Any(e => e.Id == id);
        }
    }
}