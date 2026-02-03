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
    public class ListasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public ListasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }
        // GET: api/Listas
        // Devuelve las listas para la boleta, con candidatos, dignidades y recursos multimedia
        [HttpGet]
        public async Task<ActionResult<List<Lista>>> GetListas()
        {
            try
            {
                var listas = await _context.Listas
                    .Include(l => l.Proceso)
                    .Include(l => l.Candidatos)
                        .ThenInclude(c => c.Dignidad)
                    .Include(l => l.Candidatos)
                        .ThenInclude(c => c.GaleriaMultimedia)
                    .Include(l => l.RecursosMultimedia)
                    .ToListAsync();

                return Ok(listas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener listas: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/Listas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lista>> GetLista(int id)
        {
            try
            {
                var lista = await _context.Listas
                    .Include(l => l.Proceso)
                    .Include(l => l.Candidatos)
                        .ThenInclude(c => c.Dignidad)
                    .Include(l => l.Candidatos)
                        .ThenInclude(c => c.GaleriaMultimedia)
                    .Include(l => l.RecursosMultimedia)
                    .Include(l => l.VotosRecibidos)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (lista == null)
                    return NotFound($"No se encontró la lista con ID {id}.");

                return Ok(lista);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la lista: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
        // GET: api/Listas/simple
        [HttpGet("simple")]
        public async Task<ActionResult<IEnumerable<object>>> GetListasSimple()
        {
            var listas = await _context.Listas
                .Select(l => new { l.Id, l.NombreLista })
                .ToListAsync();

            return Ok(listas);
        }


        // PUT: api/Listas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLista(int id, Lista lista)
        {
            if (id != lista.Id)
                return BadRequest("El ID de la URL no coincide con el ID de la lista.");

            _context.Entry(lista).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListaExists(id))
                    return NotFound("La lista no existe.");
                else
                    throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar lista: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/Listas
        [HttpPost]
        public async Task<ActionResult<Lista>> PostLista(Lista lista)
        {
            try
            {
                _context.Listas.Add(lista);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetLista), new { id = lista.Id }, lista);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear lista: {ex.Message}");
                return StatusCode(500, $"Error al guardar la lista: {ex.Message}");
            }
        }

        // DELETE: api/Listas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Lista>> DeleteLista(int id)
        {
            try
            {
                var lista = await _context.Listas.FindAsync(id);
                if (lista == null)
                    return NotFound("Lista no encontrada.");

                _context.Listas.Remove(lista);
                await _context.SaveChangesAsync();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar lista: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }
        // GET: api/Listas/por-proceso/1
        [HttpGet("por-proceso/{idProceso}")]
        public async Task<IActionResult> GetListasPorProceso(int idProceso)
        {

            var listas = await _context.Listas
                .Include(l => l.Candidatos)
                .ThenInclude(c => c.Dignidad)
                .Include(l => l.Candidatos)
                .ThenInclude(c => c.GaleriaMultimedia)
                .Include(l => l.RecursosMultimedia)
                .ToListAsync();

            return Ok(listas);
        }


        private bool ListaExists(int id)
        {
            return _context.Listas.Any(e => e.Id == id);
        }
    }
}