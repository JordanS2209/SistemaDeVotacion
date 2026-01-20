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
    public class TiposIdentificacionesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public TiposIdentificacionesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<TipoIdentificacion>>>> GetTiposIdentificaciones()
        {
            try
            {
                var tipos = await _context.TiposIdentificaciones.ToListAsync();
                return ApiResult<List<TipoIdentificacion>>.Ok(tipos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<TipoIdentificacion>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<TipoIdentificacion>>> GetTipoIdentificacion(int id)
        {
            try
            {
                var tipo = await _context.TiposIdentificaciones
                    .Include(t => t.Usuarios)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tipo == null)
                {
                    return ApiResult<TipoIdentificacion>.Fail("Tipo de identificación no encontrado.");
                }

                return ApiResult<TipoIdentificacion>.Ok(tipo);
            }
            catch (Exception ex)
            {
                return ApiResult<TipoIdentificacion>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<TipoIdentificacion>>> PutTipoIdentificacion(int id, TipoIdentificacion tipo)
        {
            if (id != tipo.Id)
            {
                return ApiResult<TipoIdentificacion>.Fail("ID de Tipo de identificación no coincide.");
            }

            _context.Entry(tipo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TipoIdentificacionExists(id))
                {
                    return ApiResult<TipoIdentificacion>.Fail("Tipo de identificación no encontrado.");
                }
                else
                {
                    return ApiResult<TipoIdentificacion>.Fail(ex.Message);
                }
            }

            return ApiResult<TipoIdentificacion>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<TipoIdentificacion>>> PostTipoIdentificacion(TipoIdentificacion tipo)
        {
            try
            {
                _context.TiposIdentificaciones.Add(tipo);
                await _context.SaveChangesAsync();
                return ApiResult<TipoIdentificacion>.Ok(tipo);
            }
            catch (Exception ex)
            {
                return ApiResult<TipoIdentificacion>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<TipoIdentificacion>>> DeleteTipoIdentificacion(int id)
        {
            try
            {
                var tipo = await _context.TiposIdentificaciones.FindAsync(id);
                if (tipo == null)
                {
                    return ApiResult<TipoIdentificacion>.Fail("Tipo de identificación no encontrado.");
                }

                _context.TiposIdentificaciones.Remove(tipo);
                await _context.SaveChangesAsync();

                return ApiResult<TipoIdentificacion>.Ok(tipo);
            }
            catch (Exception ex)
            {
                return ApiResult<TipoIdentificacion>.Fail(ex.Message);
            }
        }

        private bool TipoIdentificacionExists(int id)
        {
            return _context.TiposIdentificaciones.Any(e => e.Id == id);
        }
    }
}
