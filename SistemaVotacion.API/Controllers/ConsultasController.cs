using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotacion.Modelos;

namespace SistemaVotacion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public ConsultasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet("LugarVotacion/{cedula}")]
        public async Task<ActionResult<Usuario>> GetLugarVotacion(string cedula)
        {
            // Carga recursiva de todas las tablas asociadas al recinto y provincia
            var usuario = await _context.Usuarios
                .Include(u => u.PerfilesVotante)
                    .ThenInclude(v => v.Junta)
                        .ThenInclude(j => j.Recintos) // Nota: 'Recintos' según tu modelo
                            .ThenInclude(r => r.Parroquia)
                                .ThenInclude(p => p.Ciudad)
                                    .ThenInclude(c => c.Provincia)
                .FirstOrDefaultAsync(u => u.NumeroIdentificacion == cedula);

            if (usuario == null) return NotFound();
            return usuario;
        }
    }
}