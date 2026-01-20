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
    public class ProcesosElectoralesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public ProcesosElectoralesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<ProcesoElectoral>>>> GetProcesosElectorales()
        {
            try
            {
                var procesos = await _context.ProcesosElectorales.ToListAsync();
                return ApiResult<List<ProcesoElectoral>>.Ok(procesos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<ProcesoElectoral>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<ProcesoElectoral>>> GetProcesoElectoral(int id)
        {
            try
            {
                var proceso = await _context.ProcesosElectorales
                    .Include(p => p.TipoProceso)
                    .Include(p => p.PadronElectoral)
                    .Include(p => p.ListasParticipantes)
                    .Include(p => p.DignidadesAElegir)
                    .Include(p => p.PreguntasConsulta)
                    .Include(p => p.VotoDetallados)
                    .Include(p => p.RepresentantesMesas)
                    .Include(p => p.ActasGeneradas)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (proceso == null)
                {
                    return ApiResult<ProcesoElectoral>.Fail("Proceso electoral no encontrado.");
                }

                return ApiResult<ProcesoElectoral>.Ok(proceso);
            }
            catch (Exception ex)
            {
                return ApiResult<ProcesoElectoral>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<ProcesoElectoral>>> PutProcesoElectoral(int id, ProcesoElectoral proceso)
        {
            if (id != proceso.Id)
            {
                return ApiResult<ProcesoElectoral>.Fail("ID de Proceso electoral no coincide.");
            }

            _context.Entry(proceso).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProcesoElectoralExists(id))
                {
                    return ApiResult<ProcesoElectoral>.Fail("Proceso electoral no encontrado.");
                }
                else
                {
                    return ApiResult<ProcesoElectoral>.Fail(ex.Message);
                }
            }

            return ApiResult<ProcesoElectoral>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<ProcesoElectoral>>> PostProcesoElectoral(ProcesoElectoral proceso)
        {
            try
            {
                _context.ProcesosElectorales.Add(proceso);
                await _context.SaveChangesAsync();
                return ApiResult<ProcesoElectoral>.Ok(proceso);
            }
            catch (Exception ex)
            {
                return ApiResult<ProcesoElectoral>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<ProcesoElectoral>>> DeleteProcesoElectoral(int id)
        {
            try
            {
                var proceso = await _context.ProcesosElectorales.FindAsync(id);
                if (proceso == null)
                {
                    return ApiResult<ProcesoElectoral>.Fail("Proceso electoral no encontrado.");
                }

                _context.ProcesosElectorales.Remove(proceso);
                await _context.SaveChangesAsync();

                return ApiResult<ProcesoElectoral>.Ok(proceso);
            }
            catch (Exception ex)
            {
                return ApiResult<ProcesoElectoral>.Fail(ex.Message);
            }
        }

        private bool ProcesoElectoralExists(int id)
        {
            return _context.ProcesosElectorales.Any(e => e.Id == id);
        }
    }
}
