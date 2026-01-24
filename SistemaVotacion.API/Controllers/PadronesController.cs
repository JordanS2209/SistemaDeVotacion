using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotacion.Modelos;

namespace SistemaVotacion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PadronesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public PadronesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Padron>>>> GetPadrones()
        {
            try
            {
                var padrones = await _context.Padrones.ToListAsync();
                return ApiResult<List<Padron>>.Ok(padrones);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Padron>>.Fail(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Padron>>> GetPadron(int id)
        {
            try
            {
                var padron = await _context.Padrones
                    .Include(p => p.Proceso)
                    .Include(p => p.Votante)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (padron == null)
                {
                    return ApiResult<Padron>.Fail("Padron no encontrado.");
                }

                return ApiResult<Padron>.Ok(padron);
            }
            catch (Exception ex)
            {
                return ApiResult<Padron>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Padron>>> PutPadron(int id, Padron padron)
        {
            if (id != padron.Id)
            {
                return ApiResult<Padron>.Fail("ID de Padron no coincide.");
            }

            _context.Entry(padron).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PadronExists(id))
                {
                    return ApiResult<Padron>.Fail("Padron no encontrado.");
                }
                else
                {
                    return ApiResult<Padron>.Fail(ex.Message);
                }
            }

            return ApiResult<Padron>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Padron>>> PostPadron(Padron padron)
        {
            try
            {
                _context.Padrones.Add(padron);
                await _context.SaveChangesAsync();
                return ApiResult<Padron>.Ok(padron);
            }
            catch (Exception ex)
            {
                return ApiResult<Padron>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Padron>>> DeletePadron(int id)
        {
            try
            {
                var padron = await _context.Padrones.FindAsync(id);
                if (padron == null)
                {
                    return ApiResult<Padron>.Fail("Padron no encontrado.");
                }

                _context.Padrones.Remove(padron);
                await _context.SaveChangesAsync();

                return ApiResult<Padron>.Ok(padron);
            }
            catch (Exception ex)
            {
                return ApiResult<Padron>.Fail(ex.Message);
            }
        }

        private bool PadronExists(int id)
        {
            return _context.Padrones.Any(e => e.Id == id);
        }
    }
}
