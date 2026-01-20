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
    public class ListasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public ListasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Lista>>>> GetListas()
        {
            try
            {
                var listas = await _context.Listas.ToListAsync();
                return ApiResult<List<Lista>>.Ok(listas);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Lista>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<Lista>>> GetLista(int id)
        {
            try
            {
                var lista = await _context.Listas
                    .Include(l => l.Procesos)
                    .Include(l => l.Candidatos)
                    .Include(l => l.RecursosMultimedia)
                    .Include(l => l.VotosRecibidos)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (lista == null)
                {
                    return ApiResult<Lista>.Fail("Lista no encontrada.");
                }

                return ApiResult<Lista>.Ok(lista);
            }
            catch (Exception ex)
            {
                return ApiResult<Lista>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Lista>>> PutLista(int id, Lista lista)
        {
            if (id != lista.Id)
            {
                return ApiResult<Lista>.Fail("ID de Lista no coincide.");
            }

            _context.Entry(lista).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ListaExists(id))
                {
                    return ApiResult<Lista>.Fail("Lista no encontrada.");
                }
                else
                {
                    return ApiResult<Lista>.Fail(ex.Message);
                }
            }

            return ApiResult<Lista>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Lista>>> PostLista(Lista lista)
        {
            try
            {
                _context.Listas.Add(lista);
                await _context.SaveChangesAsync();
                return ApiResult<Lista>.Ok(lista);
            }
            catch (Exception ex)
            {
                return ApiResult<Lista>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Lista>>> DeleteLista(int id)
        {
            try
            {
                var lista = await _context.Listas.FindAsync(id);
                if (lista == null)
                {
                    return ApiResult<Lista>.Fail("Lista no encontrada.");
                }

                _context.Listas.Remove(lista);
                await _context.SaveChangesAsync();

                return ApiResult<Lista>.Ok(lista);
            }
            catch (Exception ex)
            {
                return ApiResult<Lista>.Fail(ex.Message);
            }
        }

        private bool ListaExists(int id)
        {
            return _context.Listas.Any(e => e.Id == id);
        }
    }
}
