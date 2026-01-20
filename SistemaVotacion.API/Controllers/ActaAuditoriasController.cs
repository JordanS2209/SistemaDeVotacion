using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotacion.Modelos;
namespace SistemaVotacion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActaAuditoriasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;
        public ActaAuditoriasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<ActaAuditoria>>>> GetActaAuditoria() 
        {
            try
            {
                var actas = await _context.ActasAuditorias.ToListAsync();
                return ApiResult<List<ActaAuditoria>>.Ok(actas);
            }
            catch (Exception ex)
            {
                return ApiResult<List<ActaAuditoria>>.Fail(ex.Message);
            }

        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<ActaAuditoria>>> GetActaAuditoria(int id)
        {
            try 
            {
                var actaAuditoria = await _context.ActasAuditorias
                    .Include(a => a.Procesos)
                    .Include(a => a.Juntas)
                    .Include(a => a.DetallesResultados)
                    .FirstOrDefaultAsync(a => a.Id == id);
                if(actaAuditoria == null)
                {
                    return ApiResult<ActaAuditoria>.Fail("Acta de Auditoria no encontrada.");
                }
                return ApiResult<ActaAuditoria>.Ok(actaAuditoria);
            }
            catch (Exception ex)
            {
                return ApiResult<ActaAuditoria>.Fail(ex.Message);
            }

        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<ActaAuditoria>>> PutActaAuditoria(int id, ActaAuditoria actaAuditoria)
        {
            if (id != actaAuditoria.Id)
            {
                return ApiResult<ActaAuditoria>.Fail("ID de Acta de Auditoria no coincide.");
            }

            _context.Entry(actaAuditoria).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ActaAuditoriaExists(id))
                {
                    return ApiResult<ActaAuditoria>.Fail("Datos de Acta de Auditoria no encontrada.");
                }
                else
                {
                   return ApiResult<ActaAuditoria>.Fail(ex.Message);
                }
            }
            return ApiResult<ActaAuditoria>.Ok(null);

        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<ActaAuditoria>>> PostActaAuditoria(ActaAuditoria actaAuditoria)
        {
            try
            {
                _context.ActasAuditorias.Add(actaAuditoria);
                await _context.SaveChangesAsync();
                return ApiResult<ActaAuditoria>.Ok(actaAuditoria);
            }
            catch (Exception ex)
            {
                return ApiResult<ActaAuditoria>.Fail(ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<ActaAuditoria>>> DeleteActaAuditoria(int id)
        {
            try
            {
                var actaAuditoria = await _context.ActasAuditorias.FindAsync(id);
                if (actaAuditoria == null)
                {
                    return ApiResult<ActaAuditoria>.Fail("Datos de Acta de Auditoria no encontrada.");
                }
                _context.ActasAuditorias.Remove(actaAuditoria);
                await _context.SaveChangesAsync();
                return ApiResult<ActaAuditoria>.Ok(actaAuditoria);
            }
            catch (Exception ex)
            {
                return ApiResult<ActaAuditoria>.Fail(ex.Message);
            }
        }
        private bool ActaAuditoriaExists(int id)
        {
            return _context.ActasAuditorias.Any(e => e.Id == id);
        }
    }

}
