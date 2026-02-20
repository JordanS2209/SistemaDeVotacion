    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class UsuarioListDto
    {
        public int Id { get; set; }

        public string Nombres { get; set; }

        public string Apellidos { get; set; }

        public string Email { get; set; }

        public string Rol { get; set; }
        
        public string NumeroIdentificacion { get; set; }

        public bool? CuentaBloqueada { get; set; }
    }

}
