using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotacion.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVotacion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MultimediasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        public MultimediasController(SistemaVotacionAPIContext context, BlobServiceClient blobServiceClient) 
        { 
            _context = context; 
            _blobServiceClient = blobServiceClient; 
        }

        // GET: api/Multimedias
        [HttpGet]
        public async Task<ActionResult<List<Multimedia>>> GetMultimedias()
        {
            try
            {
                var multimedias = await _context.Multimedias
                    .Include(m => m.Candidato)
                    .Include(m => m.Lista)
                    .ToListAsync();
                return Ok(multimedias);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener recursos multimedia: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/Multimedias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Multimedia>> GetMultimedia(int id)
        {
            try
            {
                var multimedia = await _context.Multimedias
                    .Include(m => m.Candidato)
                    .Include(m => m.Lista)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (multimedia == null)
                {
                    return NotFound($"No se encontró el recurso multimedia con ID {id}.");
                }

                return Ok(multimedia);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetMultimedia: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/Multimedias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMultimedia(int id, Multimedia multimedia)
        {
            try
            {
                if (multimedia == null)
                    return BadRequest("El cuerpo de la petición está vacío.");

                if (id != multimedia.Id)
                    return BadRequest("El ID de la URL no coincide con el ID del recurso.");

                if (string.IsNullOrWhiteSpace(multimedia.UrlFoto))
                    return BadRequest("UrlFoto es obligatorio.");

                var existente = await _context.Multimedias.FirstOrDefaultAsync(m => m.Id == id);
                if (existente == null)
                    return NotFound("El recurso multimedia no existe.");

              
                existente.UrlFoto = multimedia.UrlFoto.Trim();
                existente.Descripcion = string.IsNullOrWhiteSpace(multimedia.Descripcion)
                    ? null
                    : multimedia.Descripcion.Trim();

                await _context.SaveChangesAsync();
                return Ok("Multimedia actualizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar multimedia: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }


        // POST: api/Multimedias/upload
        [HttpPost("upload")]
        public async Task<ActionResult<Multimedia>> Upload(IFormFile file, int? idCandidato, int? idLista, string? descripcion)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Archivo no válido.");

                // exactamente uno debe venir
                var tieneCandidato = idCandidato.HasValue && idCandidato.Value > 0;
                var tieneLista = idLista.HasValue && idLista.Value > 0;

                if (tieneCandidato == tieneLista) // ambos true o ambos false
                    return BadRequest("Debe enviar IdCandidato o IdLista (solo uno).");

                // Validar 
                if (tieneCandidato)
                {
                    var existeCandidato = await _context.Candidatos
                        .AsNoTracking()
                        .AnyAsync(c => c.Id == idCandidato!.Value);

                    if (!existeCandidato)
                        return BadRequest("IdCandidato no existe.");
                }

                if (tieneLista)
                {
                    var existeLista = await _context.Listas
                        .AsNoTracking()
                        .AnyAsync(l => l.Id == idLista!.Value);

                    if (!existeLista)
                        return BadRequest("IdLista no existe.");
                }

                // Nombre único para el archivo
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                // Contenedor en Azure Blob
                var container = _blobServiceClient.GetBlobContainerClient("multimedia");
                await container.CreateIfNotExistsAsync();

                var blobClient = container.GetBlobClient(fileName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                var urlFoto = blobClient.Uri.ToString();

                var multimedia = new Multimedia
                {
                    UrlFoto = urlFoto,
                    Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim(),
                    IdCandidato = tieneCandidato ? idCandidato : null,
                    IdLista = tieneLista ? idLista : null
                };

                _context.Multimedias.Add(multimedia);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetMultimedia), new { id = multimedia.Id }, multimedia);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir multimedia: {ex.Message}");
                return StatusCode(500, $"Error al subir multimedia: {ex.Message}");
            }
        }




        // DELETE: api/Multimedias/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Multimedia>> DeleteMultimedia(int id)
        {
            try
            {
                var multimedia = await _context.Multimedias.FindAsync(id);
                if (multimedia == null)
                {
                    return NotFound("Recurso multimedia no encontrado.");
                }

                _context.Multimedias.Remove(multimedia);
                await _context.SaveChangesAsync();

                return Ok(multimedia); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar multimedia: {ex.Message}");
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool MultimediaExists(int id)
        {
            return _context.Multimedias.Any(e => e.Id == id);
        }
    }
}