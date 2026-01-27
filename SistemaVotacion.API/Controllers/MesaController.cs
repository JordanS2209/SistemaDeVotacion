using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace SistemaVotacion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MesaController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;
        public MesaController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // 1) Mesa ingresa cédula, se genera y devuelve código de 6 dígitos
        [HttpPost("autorizar-votante")]
        public async Task<IActionResult> AutorizarVotante([FromBody] AutorizarVotanteRequest request)
        {
            var ahora = DateTime.Now;

            // Proceso activo
            var procesoActivo = await _context.ProcesosElectorales
                .FirstOrDefaultAsync(p => ahora >= p.FechaInicio && ahora <= p.FechaFin);

            if (procesoActivo == null)
            {
                return BadRequest("No hay proceso electoral activo.");
            }

            // Buscar votante por número de identificación
            var votante = await _context.Votantes
                .Include(v => v.Usuario)
                .FirstOrDefaultAsync(v => v.Usuario.NumeroIdentificacion == request.NumeroIdentificacion);

            if (votante == null)
            {
                return NotFound("Votante no encontrado en el padrón.");
            }

            // Buscar registro en padrón para este proceso
            var padron = await _context.Padrones
                .FirstOrDefaultAsync(p => p.IdProceso == procesoActivo.Id && p.IdVotante == votante.Id);

            if (padron == null)
            {
                return BadRequest("El votante no está habilitado para este proceso.");
            }

            if (padron.HaVotado)
            {
                return BadRequest("El votante ya ha sufragado.");
            }

            // Generar código aleatorio de 6 dígitos
            var codigo = GenerarCodigoDeSeisDigitos();

            padron.CodigoAcceso = codigo;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                CodigoAcceso = codigo,
                Proceso = procesoActivo.NombreProceso // o los campos que quieras mostrar en pantalla
            });
        }

        private static string GenerarCodigoDeSeisDigitos()
        {
            // 000000–999999
            var valor = RandomNumberGenerator.GetInt32(0, 1_000_000);
            return valor.ToString("D6");
        }
    }

    public class AutorizarVotanteRequest
    {
        public string NumeroIdentificacion { get; set; } = string.Empty;
    }
}
