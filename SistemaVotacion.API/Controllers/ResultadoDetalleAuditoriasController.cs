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
    public class ResultadoDetalleAuditoriasController : ControllerBase
    {
        private readonly APIContext _context;

        public ResultadoDetalleAuditoriasController(APIContext context)
        {
            _context = context;
        }

        // GET: api/ResultadoDetalleAuditorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResultadoDetalleAuditoria>>> GetResultadoDetalleAuditoria()
        {
            return await _context.ResultadoDetalleAuditorias.ToListAsync();
        }

        // GET: api/ResultadoDetalleAuditorias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultadoDetalleAuditoria>> GetResultadoDetalleAuditoria(int id)
        {
            var resultadoDetalleAuditoria = await _context.ResultadoDetalleAuditorias.FindAsync(id);

            if (resultadoDetalleAuditoria == null)
            {
                return NotFound();
            }

            return resultadoDetalleAuditoria;
        }

        // PUT: api/ResultadoDetalleAuditorias/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResultadoDetalleAuditoria(int id, ResultadoDetalleAuditoria resultadoDetalleAuditoria)
        {
            if (id != resultadoDetalleAuditoria.Id)
            {
                return BadRequest();
            }

            _context.Entry(resultadoDetalleAuditoria).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResultadoDetalleAuditoriaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ResultadoDetalleAuditorias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ResultadoDetalleAuditoria>> PostResultadoDetalleAuditoria(ResultadoDetalleAuditoria resultadoDetalleAuditoria)
        {
            _context.ResultadoDetalleAuditorias.Add(resultadoDetalleAuditoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetResultadoDetalleAuditoria", new { id = resultadoDetalleAuditoria.Id }, resultadoDetalleAuditoria);
        }

        // DELETE: api/ResultadoDetalleAuditorias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResultadoDetalleAuditoria(int id)
        {
            var resultadoDetalleAuditoria = await _context.ResultadoDetalleAuditorias.FindAsync(id);
            if (resultadoDetalleAuditoria == null)
            {
                return NotFound();
            }

            _context.ResultadoDetalleAuditorias.Remove(resultadoDetalleAuditoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ResultadoDetalleAuditoriaExists(int id)
        {
            return _context.ResultadoDetalleAuditorias.Any(e => e.Id == id);
        }
    }
}
