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
        [Key] public int Id { get; set; }

        public string NombreProceso { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }


        [Required, ForeignKey("TipoProceso")]
        public int IdTipoProceso { get; set; }


        public  TipoProceso? TipoProceso { get; set; }


        public  List<Padron> PadronElectoral { get; set; } = new List<Padron>();
        public  List<Lista> ListasParticipantes { get; set; } = new List<Lista>();
        public  List<Dignidad> DignidadesAElegir { get; set; } = new List<Dignidad>();
        public  List<PreguntaConsulta> PreguntasConsulta { get; set; } = new List<PreguntaConsulta>();
        public  List<VotoDetalle> VotoDetallados { get; set; } = new List<VotoDetalle>();
        public  List<RepresentanteJunta> RepresentantesMesas { get; set; } = new List<RepresentanteJunta>();
        public  List<ActaAuditoria> ActasGeneradas { get; set; } = new List<ActaAuditoria>();
    }
}
