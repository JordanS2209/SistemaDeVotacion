using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Genero
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdGenero { get; set; }

        public string DetalleGenero { get; set; } 

        public  List<Usuario>? Usuarios { get; set; } 

        public  List<JuntaReceptora>? Juntas { get; set; } 
    }
}
