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
    public class PadronesController : ControllerBase
    {
        private readonly APIContext _context;

        public PadronesController(APIContext context)
        {
            _context = context;
        }

        // GET: api/Padrones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Padron>>> GetPadron()
        {
            return await _context.Padrones.ToListAsync();
        }

        // GET: api/Padrones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Padron>> GetPadron(int id)
        {
            var padron = await _context.Padrones.FindAsync(id);

            if (padron == null)
            {
                return NotFound();
            }

            return padron;
        }

        // PUT: api/Padrones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPadron(int id, Padron padron)
        {
            if (id != padron.Id)
            {
                return BadRequest();
            }

            _context.Entry(padron).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PadronExists(id))
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

        // POST: api/Padrones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Padron>> PostPadron(Padron padron)
        {
            _context.Padrones.Add(padron);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPadron", new { id = padron.Id }, padron);
        }

        // DELETE: api/Padrones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePadron(int id)
        {
            var padron = await _context.Padrones.FindAsync(id);
            if (padron == null)
            {
                return NotFound();
            }

            _context.Padrones.Remove(padron);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PadronExists(int id)
        {
            return _context.Padrones.Any(e => e.Id == id);
        }
    }
}
