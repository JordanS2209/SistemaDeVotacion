using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class HistorialAcceso
    {
        [Key] public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; }

        [Required]

        public string PasswordHash { get; set; }

        public int IntentosFallidos { get; set; }

        public bool? CuentaBloqueada { get; set; }

        [Required, ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        public  Usuario? Usuario { get; set; }
    }
}
