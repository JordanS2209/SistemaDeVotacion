using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class UsuarioEditDto
    {
        public int Id { get; set; }

        public string Nombres { get; set; }

        public string Apellidos { get; set; }

        public string Email { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public int IdRol { get; set; }

        public int IdTipoIdentificacion { get; set; }

        public string NumeroIdentificacion { get; set; }

        public int IdGenero { get; set; }

        public bool? CuentaBloqueada { get; set; }
    }

}
