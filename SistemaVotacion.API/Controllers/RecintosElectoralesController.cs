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
    public class RecintosElectoralesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public RecintosElectoralesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<RecintoElectoral>>>> GetRecintosElectorales()
        {
            try
            {
                var recintos = await _context.RecintosElectorales.ToListAsync();
                return ApiResult<List<RecintoElectoral>>.Ok(recintos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<RecintoElectoral>>.Fail(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<RecintoElectoral>>> GetRecintoElectoral(int id)
        {
            try
            {
                var recinto = await _context.RecintosElectorales
                    .Include(r => r.Parroquia)
                    .Include(r => r.JuntasReceptoras)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (recinto == null)
                {
                    return ApiResult<RecintoElectoral>.Fail("Recinto electoral no encontrado.");
                }

                return ApiResult<RecintoElectoral>.Ok(recinto);
            }
            catch (Exception ex)
            {
                return ApiResult<RecintoElectoral>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<RecintoElectoral>>> PutRecintoElectoral(int id, RecintoElectoral recinto)
        {
            if (id != recinto.Id)
            {
                return ApiResult<RecintoElectoral>.Fail("ID de Recinto electoral no coincide.");
            }

            _context.Entry(recinto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!RecintoElectoralExists(id))
                {
                    return ApiResult<RecintoElectoral>.Fail("Recinto electoral no encontrado.");
                }
                else
                {
                    return ApiResult<RecintoElectoral>.Fail(ex.Message);
                }
            }

            return ApiResult<RecintoElectoral>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<RecintoElectoral>>> PostRecintoElectoral(RecintoElectoral recinto)
        {
            try
            {
                _context.RecintosElectorales.Add(recinto);
                await _context.SaveChangesAsync();
                return ApiResult<RecintoElectoral>.Ok(recinto);
            }
            catch (Exception ex)
            {
                return ApiResult<RecintoElectoral>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<RecintoElectoral>>> DeleteRecintoElectoral(int id)
        {
            try
            {
                var recinto = await _context.RecintosElectorales.FindAsync(id);
                if (recinto == null)
                {
                    return ApiResult<RecintoElectoral>.Fail("Recinto electoral no encontrado.");
                }

                _context.RecintosElectorales.Remove(recinto);
                await _context.SaveChangesAsync();

                return ApiResult<RecintoElectoral>.Ok(recinto);
            }
            catch (Exception ex)
            {
                return ApiResult<RecintoElectoral>.Fail(ex.Message);
            }
        }

        private bool RecintoElectoralExists(int id)
        {
            return _context.RecintosElectorales.Any(e => e.Id == id);
        }
    }
}
