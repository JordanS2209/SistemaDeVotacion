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
    public class DignidadesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public DignidadesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Dignidad>>>> GetDignidades()
        {
            try
            {
                var dignidades = await _context.Dignidades.ToListAsync();
                return ApiResult<List<Dignidad>>.Ok(dignidades);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Dignidad>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<Dignidad>>> GetDignidad(int id)
        {
            try
            {
                var dignidad = await _context.Dignidades
                    .Include(d => d.Candidatos)
                    .Include(d => d.VotosRecibidos)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (dignidad == null)
                {
                    return ApiResult<Dignidad>.Fail("Dignidad no encontrada.");
                }

                return ApiResult<Dignidad>.Ok(dignidad);
            }
            catch (Exception ex)
            {
                return ApiResult<Dignidad>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Dignidad>>> PutDignidad(int id, Dignidad dignidad)
        {
            if (id != dignidad.Id)
            {
                return ApiResult<Dignidad>.Fail("ID de Dignidad no coincide.");
            }

            _context.Entry(dignidad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!DignidadExists(id))
                {
                    return ApiResult<Dignidad>.Fail("Dignidad no encontrada.");
                }
                else
                {
                    return ApiResult<Dignidad>.Fail(ex.Message);
                }
            }

            return ApiResult<Dignidad>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Dignidad>>> PostDignidad(Dignidad dignidad)
        {
            try
            {
                _context.Dignidades.Add(dignidad);
                await _context.SaveChangesAsync();
                return ApiResult<Dignidad>.Ok(dignidad);
            }
            catch (Exception ex)
            {
                return ApiResult<Dignidad>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Dignidad>>> DeleteDignidad(int id)
        {
            try
            {
                var dignidad = await _context.Dignidades.FindAsync(id);
                if (dignidad == null)
                {
                    return ApiResult<Dignidad>.Fail("Dignidad no encontrada.");
                }

                _context.Dignidades.Remove(dignidad);
                await _context.SaveChangesAsync();

                return ApiResult<Dignidad>.Ok(dignidad);
            }
            catch (Exception ex)
            {
                return ApiResult<Dignidad>.Fail(ex.Message);
            }
        }

        private bool DignidadExists(int id)
        {
            return _context.Dignidades.Any(e => e.Id == id);
        }
    }
}
