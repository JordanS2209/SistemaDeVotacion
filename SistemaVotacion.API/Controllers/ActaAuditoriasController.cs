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
    public class ActaAuditoriasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;
        public ActaAuditoriasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActaAuditoria>>> GetActaAuditoria() 
        {
            try
            {
                var actas = await _context.ActasAuditorias.ToListAsync();
                return Ok(actas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener Actas Auditorias: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActaAuditoria>> GetActaAuditoria(int id)
        {
            try 
            {
                var actaAuditoria = await _context.ActasAuditorias
                    .Include(a => a.Procesos)
                    .Include(a => a.Juntas)
                    .Include(a => a.DetallesResultados)
                    .FirstOrDefaultAsync(a => a.Id == id);
                if(actaAuditoria == null)
                {
                    return NotFound($"No se encontró el ActaAuditoria con ID {id}.");
                }
                return Ok(actaAuditoria);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetActaAuditoria: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutActaAuditoria(int id, ActaAuditoria actaAuditoria)
        {
            if (id != actaAuditoria.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del rol.");
            }

            _context.Entry(actaAuditoria).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ActaAuditoriaExists(id))
                {
                    return NotFound("El ActaAuditoria no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el ActaAuditoria: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ActaAuditoria>> PostActaAuditoria(ActaAuditoria actaAuditoria)
        {
            try
            {
                _context.ActasAuditorias.Add(actaAuditoria);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetActaAuditoria), new { id = actaAuditoria.Id }, actaAuditoria);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear ActaAuditoria: {ex.Message}");
                return StatusCode(500, $"Error al guardar el ActaAuditoria: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<ActaAuditoria>> DeleteActaAuditoria(int id)
        {
            try
            {
                var actaAuditoria = await _context.ActasAuditorias.FindAsync(id);
                if (actaAuditoria == null)
                {
                    return NotFound("ActaAuditoria no encontrado.");
                }
                _context.ActasAuditorias.Remove(actaAuditoria);
                await _context.SaveChangesAsync();
                return Ok(actaAuditoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el ActaAuditoria: {ex.Message}");
            }
        }
        private bool ActaAuditoriaExists(int id)
        {
            return _context.ActasAuditorias.Any(e => e.Id == id);
        }
    }

}
