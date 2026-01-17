using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class RecintoElectoral
    {
        [Key] public int Id { get; set; }
        
        public string NombreRecinto { get; set; }
        
        public string DetalleRecinto { get; set; }
        
        public string? DireccionRecinto { get; set; }


        [Required, ForeignKey("Parroquia")]
        public int IdParroquia { get; set; }

        public Parroquia? Parroquia { get; set; }

        public  List<JuntaReceptora>? JuntasReceptoras { get; set; } = new List<JuntaReceptora>();
    }
}
