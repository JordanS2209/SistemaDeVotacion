namespace SistemaVotacion.Servicios.Interfaces
{
    public interface IChartService
    {
        byte[] GeneratePieChart(dynamic data);
    }
}
