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
    public class TipoVotosController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public TipoVotosController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<TipoVoto>>>> GetTipoVotos()
        {
            try
            {
                var tipos = await _context.TipoVotos.ToListAsync();
                return ApiResult<List<TipoVoto>>.Ok(tipos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<TipoVoto>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<TipoVoto>>> GetTipoVoto(int id)
        {
            try
            {
                var tipo = await _context.TipoVotos
                    .Include(t => t.VotosAsociados)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tipo == null)
                {
                    return ApiResult<TipoVoto>.Fail("Tipo de voto no encontrado.");
                }

                return ApiResult<TipoVoto>.Ok(tipo);
            }
            catch (Exception ex)
            {
                return ApiResult<TipoVoto>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<TipoVoto>>> PutTipoVoto(int id, TipoVoto tipo)
        {
            if (id != tipo.Id)
            {
                return ApiResult<TipoVoto>.Fail("ID de Tipo de voto no coincide.");
            }

            _context.Entry(tipo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TipoVotoExists(id))
                {
                    return ApiResult<TipoVoto>.Fail("Tipo de voto no encontrado.");
                }
                else
                {
                    return ApiResult<TipoVoto>.Fail(ex.Message);
                }
            }

            return ApiResult<TipoVoto>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<TipoVoto>>> PostTipoVoto(TipoVoto tipo)
        {
            try
            {
                _context.TipoVotos.Add(tipo);
                await _context.SaveChangesAsync();
                return ApiResult<TipoVoto>.Ok(tipo);
            }
            catch (Exception ex)
            {
                return ApiResult<TipoVoto>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<TipoVoto>>> DeleteTipoVoto(int id)
        {
            try
            {
                var tipo = await _context.TipoVotos.FindAsync(id);
                if (tipo == null)
                {
                    return ApiResult<TipoVoto>.Fail("Tipo de voto no encontrado.");
                }

                _context.TipoVotos.Remove(tipo);
                await _context.SaveChangesAsync();

                return ApiResult<TipoVoto>.Ok(tipo);
            }
            catch (Exception ex)
            {
                return ApiResult<TipoVoto>.Fail(ex.Message);
            }
        }

        private bool TipoVotoExists(int id)
        {
            return _context.TipoVotos.Any(e => e.Id == id);
        }
    }
}
