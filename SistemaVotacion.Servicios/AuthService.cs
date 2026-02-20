using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;
using SistemaVotacion.Servicios.Interfaces;
using System.Security.Claims;

namespace SistemaVotacion.Servicios
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Login(string email, string password)
        {
            // Nota: Es más eficiente buscar solo el usuario por email que traer todos
            // Usamos el nuevo endpoint específico
            var usuario = Crud<Usuario>.GetSingle($"{Crud<Usuario>.EndPoint}/ByEmail/{email}");

            if (usuario != null)
            {
                // Verificamos si la cuenta está bloqueada antes de intentar el login
                if (usuario.CuentaBloqueada == true) return false;

                // BCrypt compara el texto plano con el Hash de la base de datos
                if (BCrypt.Net.BCrypt.Verify(password, usuario.ContrasenaHash))
                {
                    var datosUsuario = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Nombres),
                        new Claim(ClaimTypes.Email, usuario.Email),
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                        new Claim(ClaimTypes.Role, usuario.Rol?.NombreRol ?? "Usuario") // Agregamos el rol a la identidad
                    };

                    var credencialDigital = new ClaimsIdentity(datosUsuario, "Cookies");
                    var usuarioAutenticado = new ClaimsPrincipal(credencialDigital);

                    await _httpContextAccessor.HttpContext.SignInAsync("Cookies", usuarioAutenticado);
                    return true;
                }
            }
            return false;
        }

        // Adaptado para recibir los campos obligatorios de tu modelo
        public async Task<bool> Register(
            string email,
            string nombre,
            string apellido,
            string password,
            DateTime fechaNacimiento,
            string numeroIdentificacion,
            string codigoDactilar,
            int idRol,
            int idTipoIdentificacion,
            int idGenero)
        {
            // Verificamos duplicados con endpoints específicos
            var usuarioPorEmail = Crud<Usuario>.GetSingle($"{Crud<Usuario>.EndPoint}/ByEmail/{email}");
            var usuarioPorCedula = Crud<Usuario>.GetSingle($"{Crud<Usuario>.EndPoint}/ByCedula/{numeroIdentificacion}");

            if (usuarioPorEmail != null || usuarioPorCedula != null) return false;

            try
            {
                // CREACIÓN DEL OBJETO USUARIO CON HASH SEGURIDAD
                var nuevoUsuario = new Usuario
                {
                    Id = 0,
                    Email = email,
                    Nombres = nombre,
                    Apellidos = apellido,
                    ContrasenaHash = password,
                    FechaNacimiento = fechaNacimiento,
                    NumeroIdentificacion = numeroIdentificacion,
                    CodigoDactilar = codigoDactilar,
                    IdRol = idRol,
                    IdTipoIdentificacion = idTipoIdentificacion,
                    IdGenero = idGenero,
                    IntentosFallidos = 0,
                    CuentaBloqueada = false
                };

                Crud<Usuario>.Create(nuevoUsuario);

                //SERVICIO CORREO (No bloqueante)
                try 
                {
                    var emailService = new EmailService();
                    await emailService.EnviarCorreoRegistro(email, nombre);
                }
                catch (Exception exEmail)
                {
                    // Si falla el correo, solo lo logueamos, pero NO fallamos el registro
                    Console.WriteLine($"Advertencia: No se pudo enviar el correo de registro. Error: {exEmail.Message}");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar usuario: {ex.Message}");
                return false;
            }
        }
    }
}