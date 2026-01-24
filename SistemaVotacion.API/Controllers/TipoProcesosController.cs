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
    public class TipoProcesosController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public TipoProcesosController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<TipoProceso>>>> GetTipoProcesos()
        {
            try
            {
                var tipos = await _context.TipoProcesos.ToListAsync();
                return ApiResult<List<TipoProceso>>.Ok(tipos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<TipoProceso>>.Fail(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<TipoProceso>>> GetTipoProceso(int id)
        {
            try
            {
                var tipo = await _context.TipoProcesos
                    .Include(t => t.ProcesosAsociados)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tipo == null)
                {
                    return ApiResult<TipoProceso>.Fail("Tipo de proceso no encontrado.");
                }

                return ApiResult<TipoProceso>.Ok(tipo);
            }
            catch (Exception ex)
            {
                return ApiResult<TipoProceso>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<TipoProceso>>> PutTipoProceso(int id, TipoProceso tipo)
        {
            if (id != tipo.Id)
            {
                return ApiResult<TipoProceso>.Fail("ID de Tipo de proceso no coincide.");
            }

            _context.Entry(tipo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TipoProcesoExists(id))
                {
                    return ApiResult<TipoProceso>.Fail("Tipo de proceso no encontrado.");
                }
                else
                {
                    return ApiResult<TipoProceso>.Fail(ex.Message);
                }
            }

            return ApiResult<TipoProceso>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<TipoProceso>>> PostTipoProceso(TipoProceso tipo)
        {
            try
            {
                _context.TipoProcesos.Add(tipo);
                await _context.SaveChangesAsync();
                return ApiResult<TipoProceso>.Ok(tipo);
            }
            catch (Exception ex)
            {
                return ApiResult<TipoProceso>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<TipoProceso>>> DeleteTipoProceso(int id)
        {
            try
            {
                var tipo = await _context.TipoProcesos.FindAsync(id);
                if (tipo == null)
                {
                    return ApiResult<TipoProceso>.Fail("Tipo de proceso no encontrado.");
                }

                _context.TipoProcesos.Remove(tipo);
                await _context.SaveChangesAsync();

                return ApiResult<TipoProceso>.Ok(tipo);
            }
            catch (Exception ex)
            {
                return ApiResult<TipoProceso>.Fail(ex.Message);
            }
        }

        private bool TipoProcesoExists(int id)
        {
            return _context.TipoProcesos.Any(e => e.Id == id);
        }
    }
}
