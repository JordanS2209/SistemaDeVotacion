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
    public class VotoDetallesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public VotoDetallesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/VotoDetalles
        [HttpGet]
        public async Task<ActionResult<List<VotoDetalle>>> GetVotoDetalles()
        {
            try
            {

                var votos = await _context.VotoDetalles
                    .Include(v => v.TipoVoto)
                    .Include(v => v.Junta)
                    .Include(v => v.Dignidad)
                    .ToListAsync();
                return Ok(votos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener detalles de votos: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/VotoDetalles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VotoDetalle>> GetVotoDetalle(int id)
        {
            try
            {
                var voto = await _context.VotoDetalles
                    .Include(v => v.TipoVoto)
                    .Include(v => v.Junta)
                    .Include(v => v.Proceso)
                    .Include(v => v.Lista)
                    .Include(v => v.Dignidad)
                    .Include(v => v.Opcion)
                    .Include(v => v.Pregunta)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (voto == null)
                {
                    return NotFound($"No se encontró el detalle de voto con ID {id}.");
                }

                return Ok(voto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetVotoDetalle: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/VotoDetalles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVotoDetalle(int id, VotoDetalle voto)
        {
            if (id != voto.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del registro de voto.");
            }

            _context.Entry(voto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VotoDetalleExists(id))
                {
                    return NotFound("El detalle de voto no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar detalle de voto: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/VotoDetalles
        [HttpPost]
        public async Task<ActionResult<VotoDetalle>> PostVotoDetalle(VotoDetalle voto)
        {
            try
            {
                _context.VotoDetalles.Add(voto);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetVotoDetalle), new { id = voto.Id }, voto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear detalle de voto: {ex.Message}");
                return StatusCode(500, $"Error al guardar: {ex.Message}");
            }
        }

        // DELETE: api/VotoDetalles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<VotoDetalle>> DeleteVotoDetalle(int id)
        {
            try
            {
                var voto = await _context.VotoDetalles.FindAsync(id);
                if (voto == null)
                {
                    return NotFound("Detalle de voto no encontrado.");
                }

                _context.VotoDetalles.Remove(voto);
                await _context.SaveChangesAsync();

                return Ok(voto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar detalle de voto: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }
        [HttpPost("registrar-voto")]
        public async Task<IActionResult> RegistrarVoto([FromBody] VotoDetalle voto)
        {

            var padron = await _context.Padrones
                .FirstOrDefaultAsync(p =>
                    p.IdProceso == voto.IdProceso &&
                    !p.HaVotado
                );

            if (padron == null)
                return BadRequest("El votante ya sufragó o no es válido.");


            _context.VotoDetalles.Add(voto);


            padron.HaVotado = true;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Voto registrado correctamente" });
        }

        private bool VotoDetalleExists(int id)
        {
            return _context.VotoDetalles.Any(e => e.Id == id);
        }
    }
}