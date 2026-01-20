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
    public class ProvinciasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public ProvinciasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Provincia>>>> GetProvincias()
        {
            try
            {
                var provincias = await _context.Provincias.ToListAsync();
                return ApiResult<List<Provincia>>.Ok(provincias);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Provincia>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<Provincia>>> GetProvincia(int id)
        {
            try
            {
                var provincia = await _context.Provincias
                    .Include(p => p.Ciudades)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (provincia == null)
                {
                    return ApiResult<Provincia>.Fail("Provincia no encontrada.");
                }

                return ApiResult<Provincia>.Ok(provincia);
            }
            catch (Exception ex)
            {
                return ApiResult<Provincia>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Provincia>>> PutProvincia(int id, Provincia provincia)
        {
            if (id != provincia.Id)
            {
                return ApiResult<Provincia>.Fail("ID de Provincia no coincide.");
            }

            _context.Entry(provincia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProvinciaExists(id))
                {
                    return ApiResult<Provincia>.Fail("Provincia no encontrada.");
                }
                else
                {
                    return ApiResult<Provincia>.Fail(ex.Message);
                }
            }

            return ApiResult<Provincia>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Provincia>>> PostProvincia(Provincia provincia)
        {
            try
            {
                _context.Provincias.Add(provincia);
                await _context.SaveChangesAsync();
                return ApiResult<Provincia>.Ok(provincia);
            }
            catch (Exception ex)
            {
                return ApiResult<Provincia>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Provincia>>> DeleteProvincia(int id)
        {
            try
            {
                var provincia = await _context.Provincias.FindAsync(id);
                if (provincia == null)
                {
                    return ApiResult<Provincia>.Fail("Provincia no encontrada.");
                }

                _context.Provincias.Remove(provincia);
                await _context.SaveChangesAsync();

                return ApiResult<Provincia>.Ok(provincia);
            }
            catch (Exception ex)
            {
                return ApiResult<Provincia>.Fail(ex.Message);
            }
        }

        private bool ProvinciaExists(int id)
        {
            return _context.Provincias.Any(e => e.Id == id);
        }
    }
}
