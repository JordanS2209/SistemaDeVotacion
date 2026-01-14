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
    public class ActasAuditoriasController : ControllerBase
    {
        private readonly APIContext _context;

        public ActasAuditoriasController(APIContext context)
        {
            _context = context;
        }

        // GET: api/ActasAuditorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActaAuditoria>>> GetActaAuditoria()
        {
            return await _context.ActasAuditorias.ToListAsync();
        }

        // GET: api/ActasAuditorias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ActaAuditoria>> GetActaAuditoria(int id)
        {
            var actaAuditoria = await _context.ActasAuditorias.FindAsync(id);

            if (actaAuditoria == null)
            {
                return NotFound();
            }

            return actaAuditoria;
        }

        // PUT: api/ActasAuditorias/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActaAuditoria(int id, ActaAuditoria actaAuditoria)
        {
            if (id != actaAuditoria.Id)
            {
                return BadRequest();
            }

            _context.Entry(actaAuditoria).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActaAuditoriaExists(id))
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

        // POST: api/ActasAuditorias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ActaAuditoria>> PostActaAuditoria(ActaAuditoria actaAuditoria)
        {
            _context.ActasAuditorias.Add(actaAuditoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetActaAuditoria", new { id = actaAuditoria.Id }, actaAuditoria);
        }

        // DELETE: api/ActasAuditorias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActaAuditoria(int id)
        {
            var actaAuditoria = await _context.ActasAuditorias.FindAsync(id);
            if (actaAuditoria == null)
            {
                return NotFound();
            }

            _context.ActasAuditorias.Remove(actaAuditoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActaAuditoriaExists(int id)
        {
            return _context.ActasAuditorias.Any(e => e.Id == id);
        }
    }
}
