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
    public class ProcesosElectoralesController : ControllerBase
    {
        private readonly APIContext _context;

        public ProcesosElectoralesController(APIContext context)
        {
            _context = context;
        }

        // GET: api/ProcesosElectorales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProcesoElectoral>>> GetProcesoElectoral()
        {
            return await _context.ProcesoElectoral.ToListAsync();
        }

        // GET: api/ProcesosElectorales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProcesoElectoral>> GetProcesoElectoral(int id)
        {
            var procesoElectoral = await _context.ProcesoElectoral.FindAsync(id);

            if (procesoElectoral == null)
            {
                return NotFound();
            }

            return procesoElectoral;
        }

        // PUT: api/ProcesosElectorales/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProcesoElectoral(int id, ProcesoElectoral procesoElectoral)
        {
            if (id != procesoElectoral.Id)
            {
                return BadRequest();
            }

            _context.Entry(procesoElectoral).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProcesoElectoralExists(id))
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

        // POST: api/ProcesosElectorales
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProcesoElectoral>> PostProcesoElectoral(ProcesoElectoral procesoElectoral)
        {
            _context.ProcesoElectoral.Add(procesoElectoral);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProcesoElectoral", new { id = procesoElectoral.Id }, procesoElectoral);
        }

        // DELETE: api/ProcesosElectorales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProcesoElectoral(int id)
        {
            var procesoElectoral = await _context.ProcesoElectoral.FindAsync(id);
            if (procesoElectoral == null)
            {
                return NotFound();
            }

            _context.ProcesoElectoral.Remove(procesoElectoral);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProcesoElectoralExists(int id)
        {
            return _context.ProcesoElectoral.Any(e => e.Id == id);
        }
    }
}
