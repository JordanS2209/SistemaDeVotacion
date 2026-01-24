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
    public class ResultadosDetallesAuditoriasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public ResultadosDetallesAuditoriasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<ResultadoDetalleAuditoria>>>> GetResultadosDetallesAuditorias()
        {
            try
            {
                var resultados = await _context.ResultadosDetallesAuditorias.ToListAsync();
                return ApiResult<List<ResultadoDetalleAuditoria>>.Ok(resultados);
            }
            catch (Exception ex)
            {
                return ApiResult<List<ResultadoDetalleAuditoria>>.Fail(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<ResultadoDetalleAuditoria>>> GetResultadoDetalleAuditoria(int id)
        {
            try
            {
                var resultado = await _context.ResultadosDetallesAuditorias
                    .Include(r => r.Acta)
                    .Include(r => r.Lista)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (resultado == null)
                {
                    return ApiResult<ResultadoDetalleAuditoria>.Fail("Resultado de auditoría no encontrado.");
                }

                return ApiResult<ResultadoDetalleAuditoria>.Ok(resultado);
            }
            catch (Exception ex)
            {
                return ApiResult<ResultadoDetalleAuditoria>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<ResultadoDetalleAuditoria>>> PutResultadoDetalleAuditoria(int id, ResultadoDetalleAuditoria resultado)
        {
            if (id != resultado.Id)
            {
                return ApiResult<ResultadoDetalleAuditoria>.Fail("ID de Resultado de auditoría no coincide.");
            }

            _context.Entry(resultado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ResultadoDetalleAuditoriaExists(id))
                {
                    return ApiResult<ResultadoDetalleAuditoria>.Fail("Resultado de auditoría no encontrado.");
                }
                else
                {
                    return ApiResult<ResultadoDetalleAuditoria>.Fail(ex.Message);
                }
            }

            return ApiResult<ResultadoDetalleAuditoria>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<ResultadoDetalleAuditoria>>> PostResultadoDetalleAuditoria(ResultadoDetalleAuditoria resultado)
        {
            try
            {
                _context.ResultadosDetallesAuditorias.Add(resultado);
                await _context.SaveChangesAsync();
                return ApiResult<ResultadoDetalleAuditoria>.Ok(resultado);
            }
            catch (Exception ex)
            {
                return ApiResult<ResultadoDetalleAuditoria>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<ResultadoDetalleAuditoria>>> DeleteResultadoDetalleAuditoria(int id)
        {
            try
            {
                var resultado = await _context.ResultadosDetallesAuditorias.FindAsync(id);
                if (resultado == null)
                {
                    return ApiResult<ResultadoDetalleAuditoria>.Fail("Resultado de auditoría no encontrado.");
                }

                _context.ResultadosDetallesAuditorias.Remove(resultado);
                await _context.SaveChangesAsync();

                return ApiResult<ResultadoDetalleAuditoria>.Ok(resultado);
            }
            catch (Exception ex)
            {
                return ApiResult<ResultadoDetalleAuditoria>.Fail(ex.Message);
            }
        }

        private bool ResultadoDetalleAuditoriaExists(int id)
        {
            return _context.ResultadosDetallesAuditorias.Any(e => e.Id == id);
        }
    }
}
