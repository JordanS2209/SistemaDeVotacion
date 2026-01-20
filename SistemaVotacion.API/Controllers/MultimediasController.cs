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
    public class MultimediasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public MultimediasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Multimedia>>>> GetMultimedias()
        {
            try
            {
                var multimedias = await _context.Multimedias.ToListAsync();
                return ApiResult<List<Multimedia>>.Ok(multimedias);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Multimedia>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<Multimedia>>> GetMultimedia(int id)
        {
            try
            {
                var multimedia = await _context.Multimedias
                    .Include(m => m.Candidato)
                    .Include(m => m.Lista)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (multimedia == null)
                {
                    return ApiResult<Multimedia>.Fail("Multimedia no encontrada.");
                }

                return ApiResult<Multimedia>.Ok(multimedia);
            }
            catch (Exception ex)
            {
                return ApiResult<Multimedia>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Multimedia>>> PutMultimedia(int id, Multimedia multimedia)
        {
            if (id != multimedia.Id)
            {
                return ApiResult<Multimedia>.Fail("ID de Multimedia no coincide.");
            }

            _context.Entry(multimedia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!MultimediaExists(id))
                {
                    return ApiResult<Multimedia>.Fail("Multimedia no encontrada.");
                }
                else
                {
                    return ApiResult<Multimedia>.Fail(ex.Message);
                }
            }

            return ApiResult<Multimedia>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Multimedia>>> PostMultimedia(Multimedia multimedia)
        {
            try
            {
                _context.Multimedias.Add(multimedia);
                await _context.SaveChangesAsync();
                return ApiResult<Multimedia>.Ok(multimedia);
            }
            catch (Exception ex)
            {
                return ApiResult<Multimedia>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Multimedia>>> DeleteMultimedia(int id)
        {
            try
            {
                var multimedia = await _context.Multimedias.FindAsync(id);
                if (multimedia == null)
                {
                    return ApiResult<Multimedia>.Fail("Multimedia no encontrada.");
                }

                _context.Multimedias.Remove(multimedia);
                await _context.SaveChangesAsync();

                return ApiResult<Multimedia>.Ok(multimedia);
            }
            catch (Exception ex)
            {
                return ApiResult<Multimedia>.Fail(ex.Message);
            }
        }

        private bool MultimediaExists(int id)
        {
            return _context.Multimedias.Any(e => e.Id == id);
        }
    }
}
