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
    public class ParroquiasController : ControllerBase
    {
        private readonly APIContext _context;

        public ParroquiasController(APIContext context)
        {
            _context = context;
        }

        // GET: api/Parroquias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Parroquia>>> GetParroquia()
        {
            return await _context.Parroquias.ToListAsync();
        }

        // GET: api/Parroquias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Parroquia>> GetParroquia(int id)
        {
            var parroquia = await _context.Parroquias.FindAsync(id);

            if (parroquia == null)
            {
                return NotFound();
            }

            return parroquia;
        }

        // PUT: api/Parroquias/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParroquia(int id, Parroquia parroquia)
        {
            if (id != parroquia.Id)
            {
                return BadRequest();
            }

            _context.Entry(parroquia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParroquiaExists(id))
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

        // POST: api/Parroquias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Parroquia>> PostParroquia(Parroquia parroquia)
        {
            _context.Parroquias.Add(parroquia);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetParroquia", new { id = parroquia.Id }, parroquia);
        }

        // DELETE: api/Parroquias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParroquia(int id)
        {
            var parroquia = await _context.Parroquias.FindAsync(id);
            if (parroquia == null)
            {
                return NotFound();
            }

            _context.Parroquias.Remove(parroquia);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ParroquiaExists(int id)
        {
            return _context.Parroquias.Any(e => e.Id == id);
        }
    }
}
