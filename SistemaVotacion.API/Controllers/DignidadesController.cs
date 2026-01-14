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
    public class DignidadesController : ControllerBase
    {
        private readonly APIContext _context;

        public DignidadesController(APIContext context)
        {
            _context = context;
        }

        // GET: api/Dignidades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dignidad>>> GetDignidad()
        {
            return await _context.Dignidades.ToListAsync();
        }

        // GET: api/Dignidades/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Dignidad>> GetDignidad(int id)
        {
            var dignidad = await _context.Dignidades.FindAsync(id);

            if (dignidad == null)
            {
                return NotFound();
            }

            return dignidad;
        }

        // PUT: api/Dignidades/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDignidad(int id, Dignidad dignidad)
        {
            if (id != dignidad.Id)
            {
                return BadRequest();
            }

            _context.Entry(dignidad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DignidadExists(id))
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

        // POST: api/Dignidades
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Dignidad>> PostDignidad(Dignidad dignidad)
        {
            _context.Dignidades.Add(dignidad);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDignidad", new { id = dignidad.Id }, dignidad);
        }

        // DELETE: api/Dignidades/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDignidad(int id)
        {
            var dignidad = await _context.Dignidades.FindAsync(id);
            if (dignidad == null)
            {
                return NotFound();
            }

            _context.Dignidades.Remove(dignidad);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DignidadExists(int id)
        {
            return _context.Dignidades.Any(e => e.Id == id);
        }
    }
}
