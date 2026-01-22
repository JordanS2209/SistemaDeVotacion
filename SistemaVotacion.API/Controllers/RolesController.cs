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
    public class RolesController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public RolesController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<List<Rol>>> GetRoles()
        {
            try
            {
                var roles = await _context.Roles.ToListAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener roles: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // GET: api/Roles/Codigo/5
        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<Rol>> GetRol(int id)
        {
            try
            {
                var rol = await _context.Roles
                    .Include(r => r.Usuarios)
                    .Include(r => r.RepresentantesConEsteRol)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (rol == null)
                {
                    return NotFound($"No se encontró el rol con ID {id}.");
                }

                return Ok(rol);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetRol: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/Roles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRol(int id, Rol rol)
        {
            if (id != rol.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del rol.");
            }

            _context.Entry(rol).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); // 204 No Content: Éxito sin datos de retorno
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolExists(id))
                {
                    return NotFound("El rol no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el rol: {ex.Message}");
            }
        }

        // POST: api/Roles
        [HttpPost]
        public async Task<ActionResult<Rol>> PostRol(Rol rol)
        {
            try
            {
                _context.Roles.Add(rol);
                await _context.SaveChangesAsync();

                // Retorna 201 Created con la ruta para obtener el nuevo rol
                return CreatedAtAction(nameof(GetRol), new { id = rol.Id }, rol);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear rol: {ex.Message}");
                return StatusCode(500, $"Error al guardar el rol: {ex.Message}");
            }
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rol>> DeleteRol(int id)
        {
            try
            {
                var rol = await _context.Roles.FindAsync(id);
                if (rol == null)
                {
                    return NotFound("Rol no encontrado.");
                }

                _context.Roles.Remove(rol);
                await _context.SaveChangesAsync();

                return Ok(rol); // Retorna el objeto eliminado
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el rol: {ex.Message}");
            }
        }

        private bool RolExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}