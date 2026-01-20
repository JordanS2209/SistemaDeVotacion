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
    public class OpcionesConsultasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public OpcionesConsultasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<OpcionConsulta>>>> GetOpcionConsultas()
        {
            try
            {
                var opciones = await _context.OpcionConsultas.ToListAsync();
                return ApiResult<List<OpcionConsulta>>.Ok(opciones);
            }
            catch (Exception ex)
            {
                return ApiResult<List<OpcionConsulta>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<OpcionConsulta>>> GetOpcionConsulta(int id)
        {
            try
            {
                var opcion = await _context.OpcionConsultas
                    .Include(o => o.Pregunta)
                    .Include(o => o.VotosRecibidos)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (opcion == null)
                {
                    return ApiResult<OpcionConsulta>.Fail("Opción de consulta no encontrada.");
                }

                return ApiResult<OpcionConsulta>.Ok(opcion);
            }
            catch (Exception ex)
            {
                return ApiResult<OpcionConsulta>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<OpcionConsulta>>> PutOpcionConsulta(int id, OpcionConsulta opcion)
        {
            if (id != opcion.Id)
            {
                return ApiResult<OpcionConsulta>.Fail("ID de Opción de consulta no coincide.");
            }

            _context.Entry(opcion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!OpcionConsultaExists(id))
                {
                    return ApiResult<OpcionConsulta>.Fail("Opción de consulta no encontrada.");
                }
                else
                {
                    return ApiResult<OpcionConsulta>.Fail(ex.Message);
                }
            }

            return ApiResult<OpcionConsulta>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<OpcionConsulta>>> PostOpcionConsulta(OpcionConsulta opcion)
        {
            try
            {
                _context.OpcionConsultas.Add(opcion);
                await _context.SaveChangesAsync();
                return ApiResult<OpcionConsulta>.Ok(opcion);
            }
            catch (Exception ex)
            {
                return ApiResult<OpcionConsulta>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<OpcionConsulta>>> DeleteOpcionConsulta(int id)
        {
            try
            {
                var opcion = await _context.OpcionConsultas.FindAsync(id);
                if (opcion == null)
                {
                    return ApiResult<OpcionConsulta>.Fail("Opción de consulta no encontrada.");
                }

                _context.OpcionConsultas.Remove(opcion);
                await _context.SaveChangesAsync();

                return ApiResult<OpcionConsulta>.Ok(opcion);
            }
            catch (Exception ex)
            {
                return ApiResult<OpcionConsulta>.Fail(ex.Message);
            }
        }

        private bool OpcionConsultaExists(int id)
        {
            return _context.OpcionConsultas.Any(e => e.Id == id);
        }
    }
}
