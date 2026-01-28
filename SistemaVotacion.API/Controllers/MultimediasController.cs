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
            if (id != multimedia.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del recurso.");
            }

            _context.Entry(multimedia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MultimediaExists(id))
                {
                    return NotFound("El recurso multimedia no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar multimedia: {ex.Message}");
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/Multimedias
        [HttpPost("upload")]
        public async Task<ActionResult<Multimedia>> Upload(IFormFile file, int idCandidato, int idLista, string descripcion)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo no válido.");

            // Nombre único para el archivo
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            // Contenedor en Azure Blob
            var container = _blobServiceClient.GetBlobContainerClient("multimedia");
            await container.CreateIfNotExistsAsync();

            var blobClient = container.GetBlobClient(fileName);

            // Subir archivo
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            // URL pública del archivo
            var urlFoto = blobClient.Uri.ToString();

            var multimedia = new Multimedia
            {
                UrlFoto = urlFoto,
                Descripcion = descripcion,
                IdCandidato = idCandidato,
                IdLista = idLista
            };

            _context.Multimedias.Add(multimedia);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMultimedia), new { id = multimedia.Id }, multimedia);
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