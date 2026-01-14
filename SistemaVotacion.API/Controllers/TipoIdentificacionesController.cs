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
    public class TipoIdentificacionesController : ControllerBase
    {
        private readonly APIContext _context;

        public TipoIdentificacionesController(APIContext context)
        {
            _context = context;
        }

        // GET: api/TipoIdentificaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoIdentificacion>>> GetTipoIdentificacion()
        {
            return await _context.TipoIdentificaciones.ToListAsync();
        }

        // GET: api/TipoIdentificaciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoIdentificacion>> GetTipoIdentificacion(int id)
        {
            var tipoIdentificacion = await _context.TipoIdentificaciones.FindAsync(id);

            if (tipoIdentificacion == null)
            {
                return NotFound();
            }

            return tipoIdentificacion;
        }

        // PUT: api/TipoIdentificaciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoIdentificacion(int id, TipoIdentificacion tipoIdentificacion)
        {
            if (id != tipoIdentificacion.Id)
            {
                return BadRequest();
            }

            _context.Entry(tipoIdentificacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoIdentificacionExists(id))
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

        // POST: api/TipoIdentificaciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TipoIdentificacion>> PostTipoIdentificacion(TipoIdentificacion tipoIdentificacion)
        {
            _context.TipoIdentificaciones.Add(tipoIdentificacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTipoIdentificacion", new { id = tipoIdentificacion.Id }, tipoIdentificacion);
        }

        // DELETE: api/TipoIdentificaciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoIdentificacion(int id)
        {
            var tipoIdentificacion = await _context.TipoIdentificaciones.FindAsync(id);
            if (tipoIdentificacion == null)
            {
                return NotFound();
            }

            _context.TipoIdentificaciones.Remove(tipoIdentificacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TipoIdentificacionExists(int id)
        {
            return _context.TipoIdentificaciones.Any(e => e.Id == id);
        }
    }
}
