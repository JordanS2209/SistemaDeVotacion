using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class ProcesoElectoral
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string NombreProceso { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }


        [Required, ForeignKey("TipoProceso")]
        public int IdTipoProceso { get; set; }


        public  TipoProceso? TipoProceso { get; set; }

        public  List<Padron>? PadronElectoral { get; set; } 
        public  List<Lista>? ListasParticipantes { get; set; }
        public  List<Dignidad>? DignidadesAElegir { get; set; } 
        public  List<PreguntaConsulta>? PreguntasConsulta { get; set; }
        public  List<VotoDetalle>? VotoDetallados { get; set; } 
        public  List<RepresentanteJunta>? RepresentantesMesas { get; set; } 
        public  List<ActaAuditoria>? ActasGeneradas { get; set; }
    }
}
