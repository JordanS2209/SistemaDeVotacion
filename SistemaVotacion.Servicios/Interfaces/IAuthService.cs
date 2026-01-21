using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Servicios.Interfaces
{
    public interface IAuthService
    {
        Task<bool> Login(string email, string password);
        Task<bool> Register(string email, string nombre, string apellido, string password);

    }
}
