using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace SistemaVotacion.API
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //Ignora restriccion de Postgres y permite ingresar Fecha sin zona horario obligatoria
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<SistemaVotacionAPIContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("SistemaVotacionAPIContext") ?? throw new InvalidOperationException("Connection string 'SistemaVotacionAPIContext' not found.")));
            //Azure Blob Service storage
            builder.Services.AddSingleton(x => 
            { var config = x.GetRequiredService<IConfiguration>(); 
                var connectionString = config.GetConnectionString("AzureStorage"); 
                return new BlobServiceClient(connectionString); });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure JSON options
            builder.Services
                .AddControllers()
                .AddNewtonsoftJson(
                    options =>
                    options.SerializerSettings.ReferenceLoopHandling
                    = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();























        }
    }
}
