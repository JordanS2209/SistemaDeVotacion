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
    public class HistorialAccesosController : ControllerBase
    {
        private readonly APIContext _context;

        public HistorialAccesosController(APIContext context)
        {
            _context = context;
        }

        // GET: api/HistorialAccesos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistorialAcceso>>> GetHistorialAcceso()
        {
            return await _context.HistorialAccesos.ToListAsync();
        }

        // GET: api/HistorialAccesos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HistorialAcceso>> GetHistorialAcceso(int id)
        {
            var historialAcceso = await _context.HistorialAccesos.FindAsync(id);

            if (historialAcceso == null)
            {
                return NotFound();
            }

            return historialAcceso;
        }

        // PUT: api/HistorialAccesos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHistorialAcceso(int id, HistorialAcceso historialAcceso)
        {
            if (id != historialAcceso.Id)
            {
                return BadRequest();
            }

            _context.Entry(historialAcceso).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HistorialAccesoExists(id))
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

        // POST: api/HistorialAccesos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HistorialAcceso>> PostHistorialAcceso(HistorialAcceso historialAcceso)
        {
            _context.HistorialAccesos.Add(historialAcceso);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHistorialAcceso", new { id = historialAcceso.Id }, historialAcceso);
        }

        // DELETE: api/HistorialAccesos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistorialAcceso(int id)
        {
            var historialAcceso = await _context.HistorialAccesos.FindAsync(id);
            if (historialAcceso == null)
            {
                return NotFound();
            }

            _context.HistorialAccesos.Remove(historialAcceso);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HistorialAccesoExists(int id)
        {
            return _context.HistorialAccesos.Any(e => e.Id == id);
        }
    }
}
