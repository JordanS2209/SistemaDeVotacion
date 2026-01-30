using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotacion.Modelos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVotacion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultadosController : ControllerBase
    {
        private readonly SistemaVotacionAPIContext _context;

        public ResultadosController(SistemaVotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/Resultados/por-lista
        [HttpGet("por-lista")]
        public async Task<ActionResult<List<object>>> GetResultadosPorLista()
        {

            var votosPorLista = await _context.VotoDetalles
                .GroupBy(v => v.IdLista)
                .Select(g => new
                {
                    IdLista = g.Key,
                    TotalVotos = g.Count()
                })
                .ToListAsync();


            var resultados = await _context.Listas
                .Where(l => votosPorLista.Select(v => v.IdLista).Contains(l.Id))
                .Select(l => new
                {
                    idLista = l.Id,
                    nombreLista = l.NombreLista,
                    numeroLista = l.NumeroLista,
                    totalVotos = votosPorLista.First(v => v.IdLista == l.Id).TotalVotos
                })
                .OrderByDescending(r => r.totalVotos)
                .ToListAsync();

            return Ok(resultados);
        }
    }
}