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
    public class RepresentantesJuntasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public RepresentantesJuntasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<RepresentanteJunta>>>> GetRepresentantesJuntas()
        {
            try
            {
                var representantes = await _context.RepresentantesJuntas.ToListAsync();
                return ApiResult<List<RepresentanteJunta>>.Ok(representantes);
            }
            catch (Exception ex)
            {
                return ApiResult<List<RepresentanteJunta>>.Fail(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<RepresentanteJunta>>> GetRepresentanteJunta(int id)
        {
            try
            {
                var representante = await _context.RepresentantesJuntas
                    .Include(r => r.Usuario)
                    .Include(r => r.Junta)
                    .Include(r => r.Rol)
                    .Include(r => r.Proceso)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (representante == null)
                {
                    return ApiResult<RepresentanteJunta>.Fail("Representante de Junta no encontrado.");
                }

                return ApiResult<RepresentanteJunta>.Ok(representante);
            }
            catch (Exception ex)
            {
                return ApiResult<RepresentanteJunta>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<RepresentanteJunta>>> PutRepresentanteJunta(int id, RepresentanteJunta representante)
        {
            if (id != representante.Id)
            {
                return ApiResult<RepresentanteJunta>.Fail("ID de Representante de Junta no coincide.");
            }

            _context.Entry(representante).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!RepresentanteJuntaExists(id))
                {
                    return ApiResult<RepresentanteJunta>.Fail("Representante de Junta no encontrado.");
                }
                else
                {
                    return ApiResult<RepresentanteJunta>.Fail(ex.Message);
                }
            }

            return ApiResult<RepresentanteJunta>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<RepresentanteJunta>>> PostRepresentanteJunta(RepresentanteJunta representante)
        {
            try
            {
                _context.RepresentantesJuntas.Add(representante);
                await _context.SaveChangesAsync();
                return ApiResult<RepresentanteJunta>.Ok(representante);
            }
            catch (Exception ex)
            {
                return ApiResult<RepresentanteJunta>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<RepresentanteJunta>>> DeleteRepresentanteJunta(int id)
        {
            try
            {
                var representante = await _context.RepresentantesJuntas.FindAsync(id);
                if (representante == null)
                {
                    return ApiResult<RepresentanteJunta>.Fail("Representante de Junta no encontrado.");
                }

                _context.RepresentantesJuntas.Remove(representante);
                await _context.SaveChangesAsync();

                return ApiResult<RepresentanteJunta>.Ok(representante);
            }
            catch (Exception ex)
            {
                return ApiResult<RepresentanteJunta>.Fail(ex.Message);
            }
        }

        private bool RepresentanteJuntaExists(int id)
        {
            return _context.RepresentantesJuntas.Any(e => e.Id == id);
        }
    }
}
