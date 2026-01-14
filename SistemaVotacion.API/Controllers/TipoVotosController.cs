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
    public class TipoVotosController : ControllerBase
    {
        private readonly APIContext _context;

        public TipoVotosController(APIContext context)
        {
            _context = context;
        }

        // GET: api/TipoVotos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoVoto>>> GetTipoVoto()
        {
            return await _context.TipoVotos.ToListAsync();
        }

        // GET: api/TipoVotos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoVoto>> GetTipoVoto(int id)
        {
            var tipoVoto = await _context.TipoVotos.FindAsync(id);

            if (tipoVoto == null)
            {
                return NotFound();
            }

            return tipoVoto;
        }

        // PUT: api/TipoVotos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoVoto(int id, TipoVoto tipoVoto)
        {
            if (id != tipoVoto.Id)
            {
                return BadRequest();
            }

            _context.Entry(tipoVoto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoVotoExists(id))
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

        // POST: api/TipoVotos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TipoVoto>> PostTipoVoto(TipoVoto tipoVoto)
        {
            _context.TipoVotos.Add(tipoVoto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTipoVoto", new { id = tipoVoto.Id }, tipoVoto);
        }

        // DELETE: api/TipoVotos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoVoto(int id)
        {
            var tipoVoto = await _context.TipoVotos.FindAsync(id);
            if (tipoVoto == null)
            {
                return NotFound();
            }

            _context.TipoVotos.Remove(tipoVoto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TipoVotoExists(int id)
        {
            return _context.TipoVotos.Any(e => e.Id == id);
        }
    }
}
