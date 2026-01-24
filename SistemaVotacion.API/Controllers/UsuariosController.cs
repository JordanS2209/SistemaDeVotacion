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
    public class UsuariosController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public UsuariosController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> GetUsuarios()
        {
            try
            {
                var usuarios = await _context.Usuarios
                    .Include(u => u.Rol)
                    .Include(u => u.TipoIdentificacion)
                    .Include(u => u.Genero)
                    .ToListAsync();

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.TipoIdentificacion)
                    .Include(u => u.Rol)
                    .Include(u => u.Genero)
                    .Include(u => u.PerfilesVotante)
                    .Include(u => u.FuncionesComoRepresentante)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (usuario == null)
                {
                    return NotFound($"No se encontró el usuario con ID {id}.");
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuario {id}: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del usuario.");
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); // Respuesta estándar para una actualización exitosa sin retorno de datos
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound("El usuario no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        // POST: api/Usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            try
            {
                // Hasheamos la contraseña antes de guardarla
                usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(usuario.ContrasenaHash);

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Respuesta estándar 201 Created que indica la ubicación del nuevo recurso
                return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear usuario: {ex.Message}");
                return StatusCode(500, $"Error al guardar: {ex.Message}");
            }
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Usuario>> DeleteUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                return Ok(usuario); // Retornamos el usuario eliminado
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}