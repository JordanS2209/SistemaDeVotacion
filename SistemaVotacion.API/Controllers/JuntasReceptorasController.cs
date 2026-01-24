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
    public class JuntasReceptorasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public JuntasReceptorasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<JuntaReceptora>>>> GetJuntasReceptoras()
        {
            try
            {
                var juntas = await _context.JuntasReceptoras.ToListAsync();
                return ApiResult<List<JuntaReceptora>>.Ok(juntas);
            }
            catch (Exception ex)
            {
                return ApiResult<List<JuntaReceptora>>.Fail(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<JuntaReceptora>>> GetJuntaReceptora(int id)
        {
            try
            {
                var junta = await _context.JuntasReceptoras
                    .Include(j => j.Genero)
                    .Include(j => j.Recintos)
                    .Include(j => j.VotantesAsignados)
                    .Include(j => j.Representantes)
                    .Include(j => j.VotosRecibidos)
                    .Include(j => j.ActasCierre)
                    .FirstOrDefaultAsync(j => j.Id == id);

                if (junta == null)
                {
                    return ApiResult<JuntaReceptora>.Fail("Junta Receptora no encontrada.");
                }

                return ApiResult<JuntaReceptora>.Ok(junta);
            }
            catch (Exception ex)
            {
                return ApiResult<JuntaReceptora>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<JuntaReceptora>>> PutJuntaReceptora(int id, JuntaReceptora junta)
        {
            if (id != junta.Id)
            {
                return ApiResult<JuntaReceptora>.Fail("ID de Junta Receptora no coincide.");
            }

            _context.Entry(junta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!JuntaReceptoraExists(id))
                {
                    return ApiResult<JuntaReceptora>.Fail("Junta Receptora no encontrada.");
                }
                else
                {
                    return ApiResult<JuntaReceptora>.Fail(ex.Message);
                }
            }

            return ApiResult<JuntaReceptora>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<JuntaReceptora>>> PostJuntaReceptora(JuntaReceptora junta)
        {
            try
            {
                _context.JuntasReceptoras.Add(junta);
                await _context.SaveChangesAsync();
                return ApiResult<JuntaReceptora>.Ok(junta);
            }
            catch (Exception ex)
            {
                return ApiResult<JuntaReceptora>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<JuntaReceptora>>> DeleteJuntaReceptora(int id)
        {
            try
            {
                var junta = await _context.JuntasReceptoras.FindAsync(id);
                if (junta == null)
                {
                    return ApiResult<JuntaReceptora>.Fail("Junta Receptora no encontrada.");
                }

                _context.JuntasReceptoras.Remove(junta);
                await _context.SaveChangesAsync();

                return ApiResult<JuntaReceptora>.Ok(junta);
            }
            catch (Exception ex)
            {
                return ApiResult<JuntaReceptora>.Fail(ex.Message);
            }
        }

        private bool JuntaReceptoraExists(int id)
        {
            return _context.JuntasReceptoras.Any(e => e.Id == id);
        }
    }
}
