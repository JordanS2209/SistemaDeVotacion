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

        public DateTime FechaNacimiento { get; set; }


        [Required, ForeignKey("Rol")]
        public int IdRol { get; set; }


        [Required, ForeignKey("TipoIdentificacion")]
        public int IdTipoIdentificacion { get; set; }

        public string NumeroIdentificacion { get; set; }


        [Required, ForeignKey("Genero")]
        public int IdGenero { get; set; }

        public string CodigoDactilar { get; set; }

        public TipoIdentificacion? TipoIdentificacion { get; set; }

        public  Rol? Rol { get; set; }

        public  Genero? Genero { get; set; }

        public  List<HistorialLogin>? HistorialLogins{ get; set; } 

        public  List<Votante>? PerfilesVotante { get; set; } 

        public  List<RepresentanteJunta>? FuncionesComoRepresentante { get; set; } 
    }
}
