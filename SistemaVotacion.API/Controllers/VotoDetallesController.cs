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
        private readonly APIContext _context;

        public VotoDetallesController(APIContext context)
        {
            _context = context;
        }

        // GET: api/VotoDetalles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VotoDetalle>>> GetVotoDetalle()
        {
            return await _context.VotoDetalles.ToListAsync();
        }

        // GET: api/VotoDetalles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VotoDetalle>> GetVotoDetalle(int id)
        {
            var votoDetalle = await _context.VotoDetalles.FindAsync(id);

            if (votoDetalle == null)
            {
                return NotFound();
            }

            return votoDetalle;
        }

        // PUT: api/VotoDetalles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVotoDetalle(int id, VotoDetalle votoDetalle)
        {
            if (id != votoDetalle.Id)
            {
                return BadRequest();
            }

            _context.Entry(votoDetalle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VotoDetalleExists(id))
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

        // POST: api/VotoDetalles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VotoDetalle>> PostVotoDetalle(VotoDetalle votoDetalle)
        {
            _context.VotoDetalles.Add(votoDetalle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVotoDetalle", new { id = votoDetalle.Id }, votoDetalle);
        }

        // DELETE: api/VotoDetalles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVotoDetalle(int id)
        {
            var votoDetalle = await _context.VotoDetalles.FindAsync(id);
            if (votoDetalle == null)
            {
                return NotFound();
            }

            _context.VotoDetalles.Remove(votoDetalle);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VotoDetalleExists(int id)
        {
            return _context.VotoDetalles.Any(e => e.Id == id);
        }
    }
}
