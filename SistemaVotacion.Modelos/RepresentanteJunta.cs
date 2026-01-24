using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class RepresentanteJunta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, ForeignKey("Usuario")]
        public int IdUsuario { get; set; }


        [Required, ForeignKey("JuntaReceptora")]
        public int IdJunta { get; set; }


        [Required, ForeignKey("Rol")]
        public int IdRol { get; set; }


        [Required, ForeignKey("ProcesoElectoral")]
        public int IdProceso { get; set; }

        public  Usuario? Usuario { get; set; }
        public  JuntaReceptora? Junta { get; set; }
        public  Rol? Rol { get; set; }
        public  ProcesoElectoral? Proceso { get; set; }
    }
}
