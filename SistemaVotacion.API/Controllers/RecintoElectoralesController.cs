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
    public class RecintoElectoralesController : ControllerBase
    {
        private readonly APIContext _context;

        public RecintoElectoralesController(APIContext context)
        {
            _context = context;
        }

        // GET: api/RecintoElectorales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecintoElectoral>>> GetRecintoElectoral()
        {
            return await _context.RecintoElectorales.ToListAsync();
        }

        // GET: api/RecintoElectorales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecintoElectoral>> GetRecintoElectoral(int id)
        {
            var recintoElectoral = await _context.RecintoElectorales.FindAsync(id);

            if (recintoElectoral == null)
            {
                return NotFound();
            }

            return recintoElectoral;
        }

        // PUT: api/RecintoElectorales/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecintoElectoral(int id, RecintoElectoral recintoElectoral)
        {
            if (id != recintoElectoral.Id)
            {
                return BadRequest();
            }

            _context.Entry(recintoElectoral).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecintoElectoralExists(id))
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

        // POST: api/RecintoElectorales
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RecintoElectoral>> PostRecintoElectoral(RecintoElectoral recintoElectoral)
        {
            _context.RecintoElectorales.Add(recintoElectoral);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecintoElectoral", new { id = recintoElectoral.Id }, recintoElectoral);
        }

        // DELETE: api/RecintoElectorales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecintoElectoral(int id)
        {
            var recintoElectoral = await _context.RecintoElectorales.FindAsync(id);
            if (recintoElectoral == null)
            {
                return NotFound();
            }

            _context.RecintoElectorales.Remove(recintoElectoral);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RecintoElectoralExists(int id)
        {
            return _context.RecintoElectorales.Any(e => e.Id == id);
        }
    }
}
