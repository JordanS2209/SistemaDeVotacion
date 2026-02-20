using Microsoft.EntityFrameworkCore;

namespace SistemaVotacion.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var builder = WebApplication.CreateBuilder(args);

            // 🔑 ÚNICA cadena de conexión
            var connectionString =
                builder.Configuration.GetConnectionString("SistemaVotacionAPIContext")
                ?? builder.Configuration["SistemaVotacionAPIContext"];

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'SistemaVotacionAPIContext'.");

            builder.Services.AddDbContext<SistemaVotacionAPIContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Swagger SIEMPRE activo
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            // Si tienes autenticación JWT/Cookies, descomenta:
            // app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}