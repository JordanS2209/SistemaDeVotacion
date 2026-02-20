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

        [HttpGet]
        public async Task<ActionResult<List<UsuarioListDto>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .Select(u => new UsuarioListDto
                {
                    Id = u.Id,
                    Nombres = u.Nombres,
                    Apellidos = u.Apellidos,
                    Email = u.Email,
                    Rol = u.Rol.NombreRol,
                    NumeroIdentificacion = u.NumeroIdentificacion,
                    CuentaBloqueada = u.CuentaBloqueada
                })
                .ToListAsync();

            return Ok(usuarios);
        }

       
        [HttpGet("ByEmail/{email}")]
        public async Task<ActionResult<Usuario>> GetUsuarioByEmail(string email)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol) // Incluimos el Rol completo
                .FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null) return NotFound();

            return Ok(usuario);
        }

        // Nuevo endpoint para Validación Registro
        [HttpGet("ByCedula/{cedula}")]
        public async Task<ActionResult<Usuario>> GetUsuarioByCedula(string cedula)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol) 
                .FirstOrDefaultAsync(u => u.NumeroIdentificacion == cedula);

            if (usuario == null) return NotFound();

            return Ok(usuario);
        }

 
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioEditDto>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            return Ok(new UsuarioEditDto
            {
                Id = usuario.Id,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Email = usuario.Email,
                FechaNacimiento = usuario.FechaNacimiento,
                IdRol = usuario.IdRol,
                IdTipoIdentificacion = usuario.IdTipoIdentificacion,
                NumeroIdentificacion = usuario.NumeroIdentificacion,
                IdGenero = usuario.IdGenero,
                CuentaBloqueada = usuario.CuentaBloqueada
            });
        }

 
        [HttpPost]
        public async Task<IActionResult> PostUsuario(Usuario usuario)
        {
            try
            {
                usuario.ContrasenaHash =
                    BCrypt.Net.BCrypt.HashPassword(usuario.ContrasenaHash);

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear usuario: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioEditDto dto)
        {
            if (id != dto.Id)
                return BadRequest("El ID no coincide.");

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            usuario.Nombres = dto.Nombres;
            usuario.Apellidos = dto.Apellidos;
            usuario.Email = dto.Email;
            usuario.FechaNacimiento = dto.FechaNacimiento;
            usuario.IdRol = dto.IdRol;
            usuario.IdTipoIdentificacion = dto.IdTipoIdentificacion;
            usuario.NumeroIdentificacion = dto.NumeroIdentificacion;
            usuario.IdGenero = dto.IdGenero;
            usuario.CuentaBloqueada = dto.CuentaBloqueada;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                    return NotFound("El usuario no existe.");

                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
