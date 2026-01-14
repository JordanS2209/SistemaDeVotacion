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
    public class JuntasReceptorasController : ControllerBase
    {
        private readonly APIContext _context;

        public JuntasReceptorasController(APIContext context)
        {
            _context = context;
        }

        // GET: api/JuntasReceptoras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JuntaReceptora>>> GetJuntaReceptora()
        {
            return await _context.JuntasReceptoras.ToListAsync();
        }

        // GET: api/JuntasReceptoras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JuntaReceptora>> GetJuntaReceptora(int id)
        {
            var juntaReceptora = await _context.JuntasReceptoras.FindAsync(id);

            if (juntaReceptora == null)
            {
                return NotFound();
            }

            return juntaReceptora;
        }

        // PUT: api/JuntasReceptoras/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJuntaReceptora(int id, JuntaReceptora juntaReceptora)
        {
            if (id != juntaReceptora.Id)
            {
                return BadRequest();
            }

            _context.Entry(juntaReceptora).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JuntaReceptoraExists(id))
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

        // POST: api/JuntasReceptoras
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<JuntaReceptora>> PostJuntaReceptora(JuntaReceptora juntaReceptora)
        {
            _context.JuntasReceptoras.Add(juntaReceptora);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJuntaReceptora", new { id = juntaReceptora.Id }, juntaReceptora);
        }

        // DELETE: api/JuntasReceptoras/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJuntaReceptora(int id)
        {
            var juntaReceptora = await _context.JuntasReceptoras.FindAsync(id);
            if (juntaReceptora == null)
            {
                return NotFound();
            }

            _context.JuntasReceptoras.Remove(juntaReceptora);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JuntaReceptoraExists(int id)
        {
            return _context.JuntasReceptoras.Any(e => e.Id == id);
        }
    }
}
