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
    public class GenerosController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public GenerosController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Genero>>>> GetGeneros()
        {
            try
            {
                var generos = await _context.Generos.ToListAsync();
                return ApiResult<List<Genero>>.Ok(generos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Genero>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<Genero>>> GetGenero(int id)
        {
            try
            {
                var genero = await _context.Generos
                    .Include(g => g.Usuarios)
                    .Include(g => g.Juntas)
                    .FirstOrDefaultAsync(g => g.IdGenero == id);

                if (genero == null)
                {
                    return ApiResult<Genero>.Fail("Genero no encontrado.");
                }

                return ApiResult<Genero>.Ok(genero);
            }
            catch (Exception ex)
            {
                return ApiResult<Genero>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Genero>>> PutGenero(int id, Genero genero)
        {
            if (id != genero.IdGenero)
            {
                return ApiResult<Genero>.Fail("ID de Genero no coincide.");
            }

            _context.Entry(genero).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!GeneroExists(id))
                {
                    return ApiResult<Genero>.Fail("Genero no encontrado.");
                }
                else
                {
                    return ApiResult<Genero>.Fail(ex.Message);
                }
            }

            return ApiResult<Genero>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Genero>>> PostGenero(Genero genero)
        {
            try
            {
                _context.Generos.Add(genero);
                await _context.SaveChangesAsync();
                return ApiResult<Genero>.Ok(genero);
            }
            catch (Exception ex)
            {
                return ApiResult<Genero>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Genero>>> DeleteGenero(int id)
        {
            try
            {
                var genero = await _context.Generos.FindAsync(id);
                if (genero == null)
                {
                    return ApiResult<Genero>.Fail("Genero no encontrado.");
                }

                _context.Generos.Remove(genero);
                await _context.SaveChangesAsync();

                return ApiResult<Genero>.Ok(genero);
            }
            catch (Exception ex)
            {
                return ApiResult<Genero>.Fail(ex.Message);
            }
        }

        private bool GeneroExists(int id)
        {
            return _context.Generos.Any(e => e.IdGenero == id);
        }
    }
}
