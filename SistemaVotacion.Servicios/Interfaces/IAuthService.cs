using System;
using System.Threading.Tasks;

namespace SistemaVotacion.Servicios.Interfaces
{
    public interface IAuthService
    {
        // Método para autenticación de Administradores y Delegados
        Task<bool> Login(string email, string password);

        // Método para registro completo de un Usuario en el sistema
        Task<bool> Register(
            string email,
            string nombre,
            string apellido,
            string password,
            DateTime fechaNacimiento,
            string numeroIdentificacion,
            string codigoDactilar,
            int idRol,
            int idTipoIdentificacion,
            int idGenero
        );
    }
}