using SistemaVotacion.Servicios.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SistemaVotacion.API;
using SistemaVotacion.ApiConsumer;
using SistemaVotacion.Modelos;
using BCrypt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


namespace SistemaVotacion.Servicios
{
    //public class AuthService : IAuthService
    //{

    //    //public async Task<bool> Login(string email, string password)
    //    //{

    //    //    var usuarios = Crud<Usuario>.GetAll();
    //    //    foreach (var usuario in usuarios)
    //    //    {
    //    //        if (usuario.Email == email)
    //    //        {

    //    //            Console.WriteLine($"Comparando pas ingresado {password} con contraseña guardada {usuario.} ");
    //    //            if (BCrypt.Net.BCrypt.Verify(password, usuario.Password))
    //    //            {
    //    //                var datosUsuario = new List<Claim>
    //    //                {
    //    //                    new Claim(ClaimTypes.Name, usuario.Nombres),
    //    //                    new Claim(ClaimTypes.Email, usuario.Email),
    //    //                };
    //    //                var credencialDigital = new ClaimsIdentity(datosUsuario, "Cookies");
    //    //                var usuarioAutenticado = new ClaimsPrincipal(credencialDigital);

    //    //                await _httpContextAccessor.HttpContext.SignInAsync("Cookies", usuarioAutenticado);
    //    //                return true;
    //    //            }
    //    //        }
    //    //    }
    //    //    return false; // Usuario no encontrado o contraseña incorrecta


    //    //}

    //    //public async Task<bool> Register(string email, string nombre, string apellido, string password)
    //    //{
    //    //    var usuarioExistente = Crud<Usuario>.GetAll()
    //    //        .FirstOrDefault(u => u.Email == email);
    //    //    if (usuarioExistente != null)
    //    //    {
    //    //        return false; // El usuario ya existe   
    //    //    }

    //    //    try
    //    //    {
    //    //        Crud<Usuario>.Create(new Usuario
    //    //        {
    //    //            Id = 0,
    //    //            Email = email,
    //    //            Password = password, // Aquí deberías aplicar un hash a la contraseña antes de guardarla
    //    //            Nombres = nombre,
    //    //            Apellidos = apellido
    //    //        });
    //    //        return true; // Registro exitoso
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        Console.WriteLine($"Error al registrar usuario: {ex.Message}");
    //    //        return false; // Error al registrar usuario

    //    //    }
    //    //}
    //}
}
