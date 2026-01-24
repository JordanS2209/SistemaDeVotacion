using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class TipoProceso
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id{ get; set; }

        public string NombreTipoProceso { get; set; }

        public string? Descripcion { get; set; }

        public  List<ProcesoElectoral>? ProcesosAsociados { get; set; } 
    }
}
