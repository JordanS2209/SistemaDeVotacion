using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Rol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public string NombreRol { get; set; }
        
        public string DescripcionRol { get; set; }

        public List<Usuario>? Usuarios { get; set; }

        public  List<RepresentanteJunta>? RepresentantesConEsteRol { get; set; } 
    }
}
