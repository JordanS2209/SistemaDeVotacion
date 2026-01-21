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

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Usuario>>>> GetUsuarios()
        {
            try
            {
                var usuarios = await _context.Usuarios.ToListAsync();
                return ApiResult<List<Usuario>>.Ok(usuarios);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Usuario>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<Usuario>>> GetUsuario(int id)
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
                    return ApiResult<Usuario>.Fail("Usuario no encontrado.");
                }

                return ApiResult<Usuario>.Ok(usuario);
            }
            catch (Exception ex)
            {
                return ApiResult<Usuario>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Usuario>>> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return ApiResult<Usuario>.Fail("ID de Usuario no coincide.");
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!UsuarioExists(id))
                {
                    return ApiResult<Usuario>.Fail("Usuario no encontrado.");
                }
                else
                {
                    return ApiResult<Usuario>.Fail(ex.Message);
                }
            }

            return ApiResult<Usuario>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Usuario>>> PostUsuario(Usuario usuario)
        {
            try
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                return ApiResult<Usuario>.Ok(usuario);
            }
            catch (Exception ex)
            {
                return ApiResult<Usuario>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Usuario>>> DeleteUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return ApiResult<Usuario>.Fail("Usuario no encontrado.");
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                return ApiResult<Usuario>.Ok(usuario);
            }
            catch (Exception ex)
            {
                return ApiResult<Usuario>.Fail(ex.Message);
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
