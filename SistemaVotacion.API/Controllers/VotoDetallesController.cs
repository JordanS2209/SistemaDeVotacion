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
    public class VotoDetallesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public VotoDetallesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<VotoDetalle>>>> GetVotoDetalles()
        {
            try
            {
                var votos = await _context.VotoDetalles.ToListAsync();
                return ApiResult<List<VotoDetalle>>.Ok(votos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<VotoDetalle>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<VotoDetalle>>> GetVotoDetalle(int id)
        {
            try
            {
                var voto = await _context.VotoDetalles
                    .Include(v => v.TipoVoto)
                    .Include(v => v.Junta)
                    .Include(v => v.Proceso)
                    .Include(v => v.Lista)
                    .Include(v => v.Dignidad)
                    .Include(v => v.Opcion)
                    .Include(v => v.Pregunta)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (voto == null)
                {
                    return ApiResult<VotoDetalle>.Fail("Detalle de voto no encontrado.");
                }

                return ApiResult<VotoDetalle>.Ok(voto);
            }
            catch (Exception ex)
            {
                return ApiResult<VotoDetalle>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<VotoDetalle>>> PutVotoDetalle(int id, VotoDetalle voto)
        {
            if (id != voto.Id)
            {
                return ApiResult<VotoDetalle>.Fail("ID de VotoDetalle no coincide.");
            }

            _context.Entry(voto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!VotoDetalleExists(id))
                {
                    return ApiResult<VotoDetalle>.Fail("Detalle de voto no encontrado.");
                }
                else
                {
                    return ApiResult<VotoDetalle>.Fail(ex.Message);
                }
            }

            return ApiResult<VotoDetalle>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<VotoDetalle>>> PostVotoDetalle(VotoDetalle voto)
        {
            try
            {
                _context.VotoDetalles.Add(voto);
                await _context.SaveChangesAsync();
                return ApiResult<VotoDetalle>.Ok(voto);
            }
            catch (Exception ex)
            {
                return ApiResult<VotoDetalle>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<VotoDetalle>>> DeleteVotoDetalle(int id)
        {
            try
            {
                var voto = await _context.VotoDetalles.FindAsync(id);
                if (voto == null)
                {
                    return ApiResult<VotoDetalle>.Fail("Detalle de voto no encontrado.");
                }

                _context.VotoDetalles.Remove(voto);
                await _context.SaveChangesAsync();

                return ApiResult<VotoDetalle>.Ok(voto);
            }
            catch (Exception ex)
            {
                return ApiResult<VotoDetalle>.Fail(ex.Message);
            }
        }

        private bool VotoDetalleExists(int id)
        {
            return _context.VotoDetalles.Any(e => e.Id == id);
        }
    }
}
