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
    public class ResultadosDetallesAuditoriasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public ResultadosDetallesAuditoriasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/ResultadosDetallesAuditorias
        [HttpGet]
        public async Task<ActionResult<List<ResultadoDetalleAuditoria>>> GetResultadosDetallesAuditorias()
        {
            try
            {
                // Incluimos el Acta y la Lista para dar contexto al detalle auditado
                var resultados = await _context.ResultadosDetallesAuditorias
                    .Include(r => r.Acta)
                    .Include(r => r.Lista)
                    .ToListAsync();
                return Ok(resultados);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener detalles de resultados de auditoría: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/ResultadosDetallesAuditorias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultadoDetalleAuditoria>> GetResultadoDetalleAuditoria(int id)
        {
            try
            {
                var resultado = await _context.ResultadosDetallesAuditorias
                    .Include(r => r.Acta)
                    .Include(r => r.Lista)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (resultado == null)
                {
                    return NotFound($"No se encontró el detalle de resultado de auditoría con ID {id}.");
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetResultadoDetalleAuditoria: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/ResultadosDetallesAuditorias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResultadoDetalleAuditoria(int id, ResultadoDetalleAuditoria resultado)
        {
            if (id != resultado.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del registro de detalle.");
            }

            _context.Entry(resultado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); // 204 No Content
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResultadoDetalleAuditoriaExists(id))
                {
                    return NotFound("El detalle de resultado de auditoría no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar detalle de auditoría: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/ResultadosDetallesAuditorias
        [HttpPost]
        public async Task<ActionResult<ResultadoDetalleAuditoria>> PostResultadoDetalleAuditoria(ResultadoDetalleAuditoria resultado)
        {
            try
            {
                _context.ResultadosDetallesAuditorias.Add(resultado);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetResultadoDetalleAuditoria), new { id = resultado.Id }, resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear detalle de auditoría: {ex.Message}");
                return StatusCode(500, $"Error al guardar el detalle: {ex.Message}");
            }
        }

        // DELETE: api/ResultadosDetallesAuditorias/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultadoDetalleAuditoria>> DeleteResultadoDetalleAuditoria(int id)
        {
            try
            {
                var resultado = await _context.ResultadosDetallesAuditorias.FindAsync(id);
                if (resultado == null)
                {
                    return NotFound("Detalle de resultado de auditoría no encontrado.");
                }

                _context.ResultadosDetallesAuditorias.Remove(resultado);
                await _context.SaveChangesAsync();

                return Ok(resultado); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar detalle de auditoría: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool ResultadoDetalleAuditoriaExists(int id)
        {
            return _context.ResultadosDetallesAuditorias.Any(e => e.Id == id);
        }
    }
}