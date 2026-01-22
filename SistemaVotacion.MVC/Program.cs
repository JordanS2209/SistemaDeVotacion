using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;
using SistemaVotacion.Servicios;
using SistemaVotacion.Servicios.Interfaces;

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



            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IAuthService, AuthService>();

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
