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
    public class PreguntasConsultasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public PreguntasConsultasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<PreguntaConsulta>>>> GetPreguntasConsultas()
        {
            try
            {
                var preguntas = await _context.PreguntasConsultas.ToListAsync();
                return ApiResult<List<PreguntaConsulta>>.Ok(preguntas);
            }
            catch (Exception ex)
            {
                return ApiResult<List<PreguntaConsulta>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<PreguntaConsulta>>> GetPreguntaConsulta(int id)
        {
            try
            {
                var pregunta = await _context.PreguntasConsultas
                    .Include(p => p.ProcesosElectorales)
                    .Include(p => p.OpcionesConsulta)
                    .Include(p => p.VotosRecibidos)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pregunta == null)
                {
                    return ApiResult<PreguntaConsulta>.Fail("Pregunta de consulta no encontrada.");
                }

                return ApiResult<PreguntaConsulta>.Ok(pregunta);
            }
            catch (Exception ex)
            {
                return ApiResult<PreguntaConsulta>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<PreguntaConsulta>>> PutPreguntaConsulta(int id, PreguntaConsulta pregunta)
        {
            if (id != pregunta.Id)
            {
                return ApiResult<PreguntaConsulta>.Fail("ID de Pregunta de consulta no coincide.");
            }

            _context.Entry(pregunta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PreguntaConsultaExists(id))
                {
                    return ApiResult<PreguntaConsulta>.Fail("Pregunta de consulta no encontrada.");
                }
                else
                {
                    return ApiResult<PreguntaConsulta>.Fail(ex.Message);
                }
            }

            return ApiResult<PreguntaConsulta>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<PreguntaConsulta>>> PostPreguntaConsulta(PreguntaConsulta pregunta)
        {
            try
            {
                _context.PreguntasConsultas.Add(pregunta);
                await _context.SaveChangesAsync();
                return ApiResult<PreguntaConsulta>.Ok(pregunta);
            }
            catch (Exception ex)
            {
                return ApiResult<PreguntaConsulta>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<PreguntaConsulta>>> DeletePreguntaConsulta(int id)
        {
            try
            {
                var pregunta = await _context.PreguntasConsultas.FindAsync(id);
                if (pregunta == null)
                {
                    return ApiResult<PreguntaConsulta>.Fail("Pregunta de consulta no encontrada.");
                }

                _context.PreguntasConsultas.Remove(pregunta);
                await _context.SaveChangesAsync();

                return ApiResult<PreguntaConsulta>.Ok(pregunta);
            }
            catch (Exception ex)
            {
                return ApiResult<PreguntaConsulta>.Fail(ex.Message);
            }
        }

        private bool PreguntaConsultaExists(int id)
        {
            return _context.PreguntasConsultas.Any(e => e.Id == id);
        }
    }
}
