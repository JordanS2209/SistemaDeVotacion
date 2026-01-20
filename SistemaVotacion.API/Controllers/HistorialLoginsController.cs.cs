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
    public class HistorialLoginsController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public HistorialLoginsController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<HistorialLogin>>>> GetHistorialLogins()
        {
            try
            {
                var historial = await _context.HistoralesLogins.ToListAsync();
                return ApiResult<List<HistorialLogin>>.Ok(historial);
            }
            catch (Exception ex)
            {
                return ApiResult<List<HistorialLogin>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<HistorialLogin>>> GetHistorialLogin(int id)
        {
            try
            {
                var login = await _context.HistoralesLogins
                    .Include(h => h.Usuario)
                    .FirstOrDefaultAsync(h => h.Id == id);

                if (login == null)
                {
                    return ApiResult<HistorialLogin>.Fail("Historial de login no encontrado.");
                }

                return ApiResult<HistorialLogin>.Ok(login);
            }
            catch (Exception ex)
            {
                return ApiResult<HistorialLogin>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<HistorialLogin>>> PutHistorialLogin(int id, HistorialLogin login)
        {
            if (id != login.Id)
            {
                return ApiResult<HistorialLogin>.Fail("ID de Historial de login no coincide.");
            }

            _context.Entry(login).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!HistorialLoginExists(id))
                {
                    return ApiResult<HistorialLogin>.Fail("Historial de login no encontrado.");
                }
                else
                {
                    return ApiResult<HistorialLogin>.Fail(ex.Message);
                }
            }

            return ApiResult<HistorialLogin>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<HistorialLogin>>> PostHistorialLogin(HistorialLogin login)
        {
            try
            {
                _context.HistoralesLogins.Add(login);
                await _context.SaveChangesAsync();
                return ApiResult<HistorialLogin>.Ok(login);
            }
            catch (Exception ex)
            {
                return ApiResult<HistorialLogin>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<HistorialLogin>>> DeleteHistorialLogin(int id)
        {
            try
            {
                var login = await _context.HistoralesLogins.FindAsync(id);
                if (login == null)
                {
                    return ApiResult<HistorialLogin>.Fail("Historial de login no encontrado.");
                }

                _context.HistoralesLogins.Remove(login);
                await _context.SaveChangesAsync();

                return ApiResult<HistorialLogin>.Ok(login);
            }
            catch (Exception ex)
            {
                return ApiResult<HistorialLogin>.Fail(ex.Message);
            }
        }

        private bool HistorialLoginExists(int id)
        {
            return _context.HistoralesLogins.Any(e => e.Id == id);
        }
    }
}
