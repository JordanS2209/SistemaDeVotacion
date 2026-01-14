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
        private readonly APIContext _context;

        public TipoProcesosController(APIContext context)
        {
            _context = context;
        }

        // GET: api/TipoProcesos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoProceso>>> GetTipoProceso()
        {
            return await _context.TipoProcesos.ToListAsync();
        }

        // GET: api/TipoProcesos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoProceso>> GetTipoProceso(int id)
        {
            var tipoProceso = await _context.TipoProcesos.FindAsync(id);

            if (tipoProceso == null)
            {
                return NotFound();
            }

            return tipoProceso;
        }

        // PUT: api/TipoProcesos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoProceso(int id, TipoProceso tipoProceso)
        {
            if (id != tipoProceso.Id)
            {
                return BadRequest();
            }

            _context.Entry(tipoProceso).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoProcesoExists(id))
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

        // POST: api/TipoProcesos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TipoProceso>> PostTipoProceso(TipoProceso tipoProceso)
        {
            _context.TipoProcesos.Add(tipoProceso);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTipoProceso", new { id = tipoProceso.Id }, tipoProceso);
        }

        // DELETE: api/TipoProcesos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoProceso(int id)
        {
            var tipoProceso = await _context.TipoProcesos.FindAsync(id);
            if (tipoProceso == null)
            {
                return NotFound();
            }

            _context.TipoProcesos.Remove(tipoProceso);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TipoProcesoExists(int id)
        {
            return _context.TipoProcesos.Any(e => e.Id == id);
        }
    }
}
