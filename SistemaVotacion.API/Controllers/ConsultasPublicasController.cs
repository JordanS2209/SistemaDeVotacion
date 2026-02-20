using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotacion.Modelos;

namespace SistemaVotacion.API.Controllers
{
    [Route("api/Consultas")]
    [ApiController]
    public class ConsultasPublicasController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public ConsultasPublicasController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet("LugarVotacion/{cedula}")]
        public async Task<IActionResult> GetLugarVotacion(string cedula)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NumeroIdentificacion == cedula);

            if (usuario == null)
            {
                return NotFound("Cédula no encontrada en el sistema.");
            }

            // 2. Buscar Votante usando el IdUsuario
            var votante = await _context.Votantes
                .Include(v => v.Junta)
                    .ThenInclude(j => j.Recintos) // Ojo: Propiedad de navegación en JuntaReceptora
                .FirstOrDefaultAsync(v => v.IdUsuario == usuario.Id);

            if (votante == null)
            {
                return NotFound("El usuario existe pero no está empadronado como votante.");
            }

            if (votante.Junta == null)
            {
                 return NotFound("Votante sin Junta asignada.");
            }

            // 3. Construir respuesta 
            var recinto = votante.Junta.Recintos; // Recintos es la propiedad de navegación a RecintoElectoral

            var resultado = new
            {
                NombreCompleto = usuario.Nombres + " " + usuario.Apellidos,
                NumJunta = votante.Junta.NumeroJunta,
                Recinto = recinto?.NombreRecinto ?? "No asignado",
                Direccion = recinto?.DireccionRecinto ?? "Sin dirección",
                Parroquia = "Consultar detalle", 
                Mesa = votante.Junta.NumeroJunta
            };

            return Ok(resultado);
        }
    }
}
