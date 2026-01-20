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
    public class CiudadesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public CiudadesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Ciudad>>>> GetCiudades()
        {
            try
            {
                var ciudades = await _context.Ciudades.ToListAsync();
                return ApiResult<List<Ciudad>>.Ok(ciudades);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Ciudad>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<Ciudad>>> GetCiudad(int id)
        {
            try
            {
                var ciudad = await _context.Ciudades
                    .Include(c => c.Provincia)
                    .Include(c => c.Parroquias)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (ciudad == null)
                {
                    return ApiResult<Ciudad>.Fail("Ciudad no encontrada.");
                }

                return ApiResult<Ciudad>.Ok(ciudad);
            }
            catch (Exception ex)
            {
                return ApiResult<Ciudad>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Ciudad>>> PutCiudad(int id, Ciudad ciudad)
        {
            if (id != ciudad.Id)
            {
                return ApiResult<Ciudad>.Fail("ID de Ciudad no coincide.");
            }

            _context.Entry(ciudad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CiudadExists(id))
                {
                    return ApiResult<Ciudad>.Fail("Ciudad no encontrada.");
                }
                else
                {
                    return ApiResult<Ciudad>.Fail(ex.Message);
                }
            }

            return ApiResult<Ciudad>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Ciudad>>> PostCiudad(Ciudad ciudad)
        {
            try
            {
                _context.Ciudades.Add(ciudad);
                await _context.SaveChangesAsync();
                return ApiResult<Ciudad>.Ok(ciudad);
            }
            catch (Exception ex)
            {
                return ApiResult<Ciudad>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Ciudad>>> DeleteCiudad(int id)
        {
            try
            {
                var ciudad = await _context.Ciudades.FindAsync(id);
                if (ciudad == null)
                {
                    return ApiResult<Ciudad>.Fail("Ciudad no encontrada.");
                }

                _context.Ciudades.Remove(ciudad);
                await _context.SaveChangesAsync();

                return ApiResult<Ciudad>.Ok(ciudad);
            }
            catch (Exception ex)
            {
                return ApiResult<Ciudad>.Fail(ex.Message);
            }
        }

        private bool CiudadExists(int id)
        {
            return _context.Ciudades.Any(e => e.Id == id);
        }
    }
}
