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

        [Required, MaxLength(100)]
        public string Nombres { get; set; }

        [Required, MaxLength(100)]

        public string Apellidos { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required, ForeignKey("Rol")]
        public int IdRol { get; set; }

        [Required, ForeignKey("TipoIdentificacion")]
        public int IdTipoIdentificacion { get; set; }

        [Required, MaxLength(25)]
        public string NumeroIdentificacion { get; set; }
        [Required, MaxLength(20)]
        public string CodigoDactilar { get; set; }

        [Required]
        public DateTime FechaExpedicion { get; set; }

        public TipoIdentificacion? TipoIdentificacion { get; set; }

        public virtual Rol? Rol { get; set; }

        public HistorialAcceso? HistorialAcceso { get; set; }

        public virtual Votante? Votante { get; set; }

    }
}
