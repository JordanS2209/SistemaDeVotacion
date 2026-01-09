using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Usuario
    {
        [Key] public int Id { get; set; }

        public string Nombres { get; set; }

        public string Apellidos { get; set; }

        public string Email { get; set; }

        public int IdRol { get; set; }

        public int IdTipoIdentificacion { get; set; }

        public string NumeroIdentificacion { get; set; }


        //public TipoIdentificacion? TipoIdentificacion { get; set; }

        public virtual Rol? Rol { get; set; }

        //public Votante? Votante { get; set; }
    }
}
