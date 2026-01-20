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
    public class ParroquiasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public ParroquiasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Parroquia>>>> GetParroquias()
        {
            try
            {
                var parroquias = await _context.Parroquias.ToListAsync();
                return ApiResult<List<Parroquia>>.Ok(parroquias);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Parroquia>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<Parroquia>>> GetParroquia(int id)
        {
            try
            {
                var parroquia = await _context.Parroquias
                    .Include(p => p.Ciudad)
                    .Include(p => p.Recintos)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (parroquia == null)
                {
                    return ApiResult<Parroquia>.Fail("Parroquia no encontrada.");
                }

                return ApiResult<Parroquia>.Ok(parroquia);
            }
            catch (Exception ex)
            {
                return ApiResult<Parroquia>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Parroquia>>> PutParroquia(int id, Parroquia parroquia)
        {
            if (id != parroquia.Id)
            {
                return ApiResult<Parroquia>.Fail("ID de Parroquia no coincide.");
            }

            _context.Entry(parroquia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ParroquiaExists(id))
                {
                    return ApiResult<Parroquia>.Fail("Parroquia no encontrada.");
                }
                else
                {
                    return ApiResult<Parroquia>.Fail(ex.Message);
                }
            }

            return ApiResult<Parroquia>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Parroquia>>> PostParroquia(Parroquia parroquia)
        {
            try
            {
                _context.Parroquias.Add(parroquia);
                await _context.SaveChangesAsync();
                return ApiResult<Parroquia>.Ok(parroquia);
            }
            catch (Exception ex)
            {
                return ApiResult<Parroquia>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Parroquia>>> DeleteParroquia(int id)
        {
            try
            {
                var parroquia = await _context.Parroquias.FindAsync(id);
                if (parroquia == null)
                {
                    return ApiResult<Parroquia>.Fail("Parroquia no encontrada.");
                }

                _context.Parroquias.Remove(parroquia);
                await _context.SaveChangesAsync();

                return ApiResult<Parroquia>.Ok(parroquia);
            }
            catch (Exception ex)
            {
                return ApiResult<Parroquia>.Fail(ex.Message);
            }
        }

        private bool ParroquiaExists(int id)
        {
            return _context.Parroquias.Any(e => e.Id == id);
        }
    }
}
