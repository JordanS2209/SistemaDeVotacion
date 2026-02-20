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
    public class CandidatosController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public CandidatosController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/Candidatos
        [HttpGet]
        public async Task<ActionResult<List<Candidato>>> GetCandidatos()
        {
            try
            {
                var data = await _context.Candidatos
                .Include(c => c.Lista)
                .Include(c => c.Dignidad)
                .Include(c => c.GaleriaMultimedia)
                .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener candidatos: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/Candidatos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Candidato>> GetCandidato(int id)
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
                    return NotFound($"No se encontró el candidato con ID {id}.");
                }
                   
                return Ok(candidato);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetCandidato: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("filtrar")]
        public async Task<ActionResult<IEnumerable<Candidato>>> Filtrar(
                string? nombre, int? lista, int? dignidad)
        {
            try
            {
                var query = _context.Candidatos
                    .Include(c => c.Lista)
                    .Include(c => c.Dignidad)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(nombre))
                    query = query.Where(c => c.NombreCandidato.Contains(nombre));

                if (lista.HasValue)
                    query = query.Where(c => c.IdLista == lista.Value);

                if (dignidad.HasValue)
                    query = query.Where(c => c.IdDignidad == dignidad.Value);

                var candidatos = await query
                    .OrderBy(c => c.NombreCandidato)
                    .ToListAsync();

                return Ok(candidatos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al filtrar candidatos: {ex.Message}");
                return StatusCode(500, $"Error interno al filtrar candidatos: {ex.Message}");
            }
        }



        // PUT: api/Candidatos/5
        [HttpPut("{id}")]
        public async Task<ActionResult> PutCandidato(int id, Candidato candidato)
        {
            try
            {
                if (candidato == null)
                    return BadRequest("El cuerpo de la petición está vacío.");

                if (id != candidato.Id)
                    return BadRequest("El ID de la URL no coincide con el ID del candidato.");

                if (string.IsNullOrWhiteSpace(candidato.NombreCandidato))
                    return BadRequest("NombreCandidato es obligatorio.");

                if (candidato.IdLista <= 0)
                    return BadRequest("IdLista es obligatorio.");

                if (candidato.IdDignidad <= 0)
                    return BadRequest("IdDignidad es obligatorio.");

                var existente = await _context.Candidatos
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (existente == null)
                    return NotFound("El candidato no existe.");

                // NO permitir mover el candidato a otra lista
                if (existente.IdLista != candidato.IdLista)
                    return BadRequest("No se puede cambiar la lista de un candidato.");

                // Validar FK Lista
                var existeLista = await _context.Listas
                    .AsNoTracking()
                    .AnyAsync(l => l.Id == candidato.IdLista);

                if (!existeLista)
                    return BadRequest("IdLista no existe.");

                // Validar FK Dignidad
                var existeDignidad = await _context.Dignidades
                    .AsNoTracking()
                    .AnyAsync(d => d.Id == candidato.IdDignidad);

                if (!existeDignidad)
                    return BadRequest("IdDignidad no existe.");

                // evitar duplicar candidato por dignidad en la misma lista
                var existeDuplicado = await _context.Candidatos
                    .AsNoTracking()
                    .AnyAsync(c =>
                        c.Id != id &&
                        c.IdLista == existente.IdLista &&
                        c.IdDignidad == candidato.IdDignidad &&
                        c.NombreCandidato.ToLower() == candidato.NombreCandidato.Trim().ToLower()
                    );

                if (existeDuplicado)
                    return Conflict("Ya existe un candidato igual para esa dignidad en esta lista.");

                existente.NombreCandidato = candidato.NombreCandidato.Trim();
                existente.IdDignidad = candidato.IdDignidad;

                await _context.SaveChangesAsync();
                return Ok("Candidato actualizado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar candidato: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<ActionResult<Candidato>> PostCandidato(Candidato candidato)
        {
            try
            {
                if (candidato == null)
                    return BadRequest("Datos inválidos.");

                if (string.IsNullOrWhiteSpace(candidato.NombreCandidato))
                    return BadRequest("NombreCandidato es obligatorio.");

                if (candidato.IdLista <= 0)
                    return BadRequest("IdLista es obligatorio.");

                if (candidato.IdDignidad <= 0)
                    return BadRequest("IdDignidad es obligatorio.");

                var existeLista = await _context.Listas.AsNoTracking().AnyAsync(l => l.Id == candidato.IdLista);
                if (!existeLista) return BadRequest("IdLista no existe.");

                var existeDignidad = await _context.Dignidades.AsNoTracking().AnyAsync(d => d.Id == candidato.IdDignidad);
                if (!existeDignidad) return BadRequest("IdDignidad no existe.");

                candidato.NombreCandidato = candidato.NombreCandidato.Trim();

                _context.Candidatos.Add(candidato);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCandidato), new { id = candidato.Id }, candidato);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear candidato: {ex.Message}");
                return StatusCode(500, $"Error al guardar el candidato: {ex.Message}");
            }
        }




        // DELETE: api/Candidatos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Candidato>> DeleteCandidato(int id)
        {
            try
            {
                var candidato = await _context.Candidatos.FindAsync(id);
                if (candidato == null)
                    return NotFound("Candidato no encontrado.");

                _context.Candidatos.Remove(candidato);
                await _context.SaveChangesAsync();

                return Ok(candidato);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar candidato: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool CandidatoExists(int id)
        {
            return _context.Candidatos.Any(e => e.Id == id);
        }
        // GET: api/Candidatos/por-lista/5
        [HttpGet("por-lista/{idLista}")]
        public async Task<ActionResult<List<Candidato>>> GetCandidatosPorLista(int idLista)
        {
            try
            {
                if (idLista <= 0)
                    return BadRequest("IdLista inválido.");

                var existeLista = await _context.Listas
                    .AsNoTracking()
                    .AnyAsync(l => l.Id == idLista);

                if (!existeLista)
                    return NotFound("La lista no existe.");

                var data = await _context.Candidatos
                    .AsNoTracking()
                    .Where(c => c.IdLista == idLista)
                    .Include(c => c.Dignidad)
                    .Include(c => c.GaleriaMultimedia)
                    .OrderBy(c => c.NombreCandidato)
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener candidatos por lista: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

    }

}
