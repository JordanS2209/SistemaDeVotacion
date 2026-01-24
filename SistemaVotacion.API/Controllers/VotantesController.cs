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
    public class VotantesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public VotantesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Votante>>>> GetVotantes()
        {
            try
            {
                var votantes = await _context.Votantes.ToListAsync();
                return ApiResult<List<Votante>>.Ok(votantes);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Votante>>.Fail(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Votante>>> GetVotante(int id)
        {
            try
            {
                var votante = await _context.Votantes
                    .Include(v => v.Usuario)
                    .Include(v => v.Junta)
                    .Include(v => v.VotantesEnPadron)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (votante == null)
                {
                    return ApiResult<Votante>.Fail("Votante no encontrado.");
                }

                return ApiResult<Votante>.Ok(votante);
            }
            catch (Exception ex)
            {
                return ApiResult<Votante>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Votante>>> PutVotante(int id, Votante votante)
        {
            if (id != votante.Id)
            {
                return ApiResult<Votante>.Fail("ID de Votante no coincide.");
            }

            _context.Entry(votante).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!VotanteExists(id))
                {
                    return ApiResult<Votante>.Fail("Votante no encontrado.");
                }
                else
                {
                    return ApiResult<Votante>.Fail(ex.Message);
                }
            }

            return ApiResult<Votante>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Votante>>> PostVotante(Votante votante)
        {
            try
            {
                _context.Votantes.Add(votante);
                await _context.SaveChangesAsync();
                return ApiResult<Votante>.Ok(votante);
            }
            catch (Exception ex)
            {
                return ApiResult<Votante>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Votante>>> DeleteVotante(int id)
        {
            try
            {
                var votante = await _context.Votantes.FindAsync(id);
                if (votante == null)
                {
                    return ApiResult<Votante>.Fail("Votante no encontrado.");
                }

                _context.Votantes.Remove(votante);
                await _context.SaveChangesAsync();

                return ApiResult<Votante>.Ok(votante);
            }
            catch (Exception ex)
            {
                return ApiResult<Votante>.Fail(ex.Message);
            }
        }

        private bool VotanteExists(int id)
        {
            return _context.Votantes.Any(e => e.Id == id);
        }
    }
}
