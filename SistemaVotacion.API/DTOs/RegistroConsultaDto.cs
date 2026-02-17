namespace SistemaVotacion.API.DTOs
{
    public class RespuestaConsultaDto
    {
        public int IdPregunta { get; set; }
        public int IdOpcion { get; set; }
    }

    public class RegistroConsultaDto
    {
        public int IdProceso { get; set; }
        public int IdPadron { get; set; }
        public List<RespuestaConsultaDto> Respuestas { get; set; } = new();
    }
}