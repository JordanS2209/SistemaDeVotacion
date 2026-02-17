namespace SistemaVotacion.API.DTOs
{
    public class ResultadoListaDto
    {
        public int IdLista { get; set; }
        public string NombreLista { get; set; }
        public int TotalVotos { get; set; }
    }

    public class ResultadoEleccionDto
    {
        public int IdProceso { get; set; }
        public string NombreProceso { get; set; }

        public int TotalVotos { get; set; }
        public string Ganador { get; set; }

        public List<ResultadoListaDto> Resultados { get; set; }
    }
}