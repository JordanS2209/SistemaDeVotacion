namespace SistemaVotacion.Servicios.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerarPdfResultadosGeneral(List<SistemaVotacion.Modelos.ResultadoGeneralDto> data, int? idProvincia);
        byte[] GenerarPdfConsultaPopular(dynamic data);
    }
}
