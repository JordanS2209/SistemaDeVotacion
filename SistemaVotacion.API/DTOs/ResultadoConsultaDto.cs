namespace SistemaVotacion.API.DTOs
{
    public class ResultadoPreguntaDto
    {
        public int IdPregunta { get; set; }
        public string TextoPregunta { get; set; }

        public int TotalSi { get; set; }
        public int TotalNo { get; set; }
        public int TotalBlancos { get; set; }

        public string Ganador { get; set; }
    }

    public class ResultadoConsultaDto
    {
        public int IdProceso { get; set; }
        public string NombreProceso { get; set; }

        public List<ResultadoPreguntaDto> Resultados { get; set; }
    }
}