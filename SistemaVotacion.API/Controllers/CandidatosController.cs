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
    public class CandidatosController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public CandidatosController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Candidato>>>> GetCandidatos()
        {
            try
            {
                var candidatos = await _context.Candidatos.ToListAsync();
                return ApiResult<List<Candidato>>.Ok(candidatos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Candidato>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<Candidato>>> GetCandidato(int id)
        {
            try
            {
                var candidato = await _context.Candidatos
                    .Include(c => c.Lista)
                    .Include(c => c.Dignidad)
                    .Include(c => c.GaleriaMultimedia)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (candidato == null)
                {
                    return ApiResult<Candidato>.Fail("Candidato no encontrado.");
                }

                return ApiResult<Candidato>.Ok(candidato);
            }
            catch (Exception ex)
            {
                return ApiResult<Candidato>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Candidato>>> PutCandidato(int id, Candidato candidato)
        {
            if (id != candidato.Id)
            {
                return ApiResult<Candidato>.Fail("ID de Candidato no coincide.");
            }

            _context.Entry(candidato).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CandidatoExists(id))
                {
                    return ApiResult<Candidato>.Fail("Datos de Candidato no encontrado.");
                }
                else
                {
                    return ApiResult<Candidato>.Fail(ex.Message);
                }
            }

            return ApiResult<Candidato>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Candidato>>> PostCandidato(Candidato candidato)
        {
            try
            {
                _context.Candidatos.Add(candidato);
                await _context.SaveChangesAsync();
                return ApiResult<Candidato>.Ok(candidato);
            }
            catch (Exception ex)
            {
                return ApiResult<Candidato>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Candidato>>> DeleteCandidato(int id)
        {
            try
            {
                var candidato = await _context.Candidatos.FindAsync(id);
                if (candidato == null)
                {
                    return ApiResult<Candidato>.Fail("Datos de Candidato no encontrado.");
                }

                _context.Candidatos.Remove(candidato);
                await _context.SaveChangesAsync();

                return ApiResult<Candidato>.Ok(candidato);
            }
            catch (Exception ex)
            {
                return ApiResult<Candidato>.Fail(ex.Message);
            }
        }

        private bool CandidatoExists(int id)
        {
            return _context.Candidatos.Any(e => e.Id == id);
        }
    }
}
