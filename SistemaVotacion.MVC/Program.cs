using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;
using SistemaVotacion.Servicios;
using SistemaVotacion.Servicios.Interfaces;
using SistemaVotacion.API;

namespace SistemaVotacion.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Crud<Usuario>.EndPoint = "https://localhost:7202/api/Usuarios";
            Crud<Rol>.EndPoint = "https://localhost:7202/api/Roles";
            Crud<TipoIdentificacion>.EndPoint = "https://localhost:7202/api/TiposIdentificaciones";
            Crud<Genero>.EndPoint = "https://localhost:7202/api/Generos";
            Crud<Dignidad>.EndPoint = "https://localhost:7202/api/Dignidades";
            Crud<Provincia>.EndPoint = "https://localhost:7202/api/Provincias";
            Crud<Ciudad>.EndPoint = "https://localhost:7202/api/Ciudades";
            Crud<Parroquia>.EndPoint = "https://localhost:7202/api/Parroquias";
            Crud<Multimedia>.EndPoint = "https://localhost:7202/api/Multimedias";
            Crud<TipoProceso>.EndPoint = "https://localhost:7202/api/TipoProcesos";
            Crud<Candidato>.EndPoint = "https://localhost:7202/api/Candidatos";
            Crud<OpcionConsulta>.EndPoint = "https://localhost:7202/api/OpcionesConsultas";
            Crud<PreguntaConsulta>.EndPoint = "https://localhost:7202/api/PreguntasConsultas";
            Crud<ProcesoElectoral>.EndPoint = "https://localhost:7202/api/ProcesosElectorales";
            Crud<Lista>.EndPoint = "https://localhost:7202/api/Boletas/activas";
            Crud<Padron>.EndPoint = "https://localhost:7202/api/Padrones";


            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddHttpClient<IMultimediaService, MultimediaService>(client => 
            { 
                client.BaseAddress = new Uri("https://localhost:7202/api/Multimedias/"); 
            });

            builder.Services.AddAuthentication("Cookies") //cokies
                            .AddCookie("Cookies", options =>
                            {
                                options.LoginPath = "/Account/Index"; // Ruta de inicio de sesión
                            });
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
