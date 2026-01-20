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

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Rol>>>> GetRoles()
        {
            try
            {
                var roles = await _context.Roles.ToListAsync();
                return ApiResult<List<Rol>>.Ok(roles);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Rol>>.Fail(ex.Message);
            }
        }

        [HttpGet("Codigo/{id}")]
        public async Task<ActionResult<ApiResult<Rol>>> GetRol(int id)
        {
            try
            {
                var rol = await _context.Roles
                    .Include(r => r.Usuarios)
                    .Include(r => r.RepresentantesConEsteRol)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (rol == null)
                {
                    return ApiResult<Rol>.Fail("Rol no encontrado.");
                }

                return ApiResult<Rol>.Ok(rol);
            }
            catch (Exception ex)
            {
                return ApiResult<Rol>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Rol>>> PutRol(int id, Rol rol)
        {
            if (id != rol.Id)
            {
                return ApiResult<Rol>.Fail("ID de Rol no coincide.");
            }

            _context.Entry(rol).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!RolExists(id))
                {
                    return ApiResult<Rol>.Fail("Rol no encontrado.");
                }
                else
                {
                    return ApiResult<Rol>.Fail(ex.Message);
                }
            }

            return ApiResult<Rol>.Ok(null);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Rol>>> PostRol(Rol rol)
        {
            try
            {
                _context.Roles.Add(rol);
                await _context.SaveChangesAsync();
                return ApiResult<Rol>.Ok(rol);
            }
            catch (Exception ex)
            {
                return ApiResult<Rol>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Rol>>> DeleteRol(int id)
        {
            try
            {
                var rol = await _context.Roles.FindAsync(id);
                if (rol == null)
                {
                    return ApiResult<Rol>.Fail("Rol no encontrado.");
                }

                _context.Roles.Remove(rol);
                await _context.SaveChangesAsync();

                return ApiResult<Rol>.Ok(rol);
            }
            catch (Exception ex)
            {
                return ApiResult<Rol>.Fail(ex.Message);
            }
        }

        private bool RolExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}
