using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Servicios.VistasModelos
{
    public class LoginViewModel
    {
        public string Usuario { get; set; } // cedula
        public string Email { get; set; } //emmail
        public string Codigo { get; set; } // codigo de verificacion
    }
}
