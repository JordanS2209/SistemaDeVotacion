using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class VotoDetalle
    {
        [Key] public int Id{ get; set; }


        [Required, ForeignKey("TipoVoto")]
        public int IdTipoVoto { get; set; }


        [Required, ForeignKey("JuntaReceptora")]
        public int IdJunta { get; set; }


        [Required, ForeignKey("ProcesoElectoral")]
        public int IdProceso { get; set; }


        [Required, ForeignKey("Lista")]
        public int? IdLista { get; set; }


        [Required, ForeignKey("Dignidad")]
        public int? IdDignidad { get; set; }


        [Required, ForeignKey("OpcionConsulta")]
        public int? IdOpcion { get; set; }


        [Required, ForeignKey("PreguntaConsulta")]
        public int? IdPregunta { get; set; }


        public  TipoVoto? TipoVoto { get; set; }
        public  JuntaReceptora? Junta { get; set; }
        public  ProcesoElectoral? Proceso { get; set; }
        public  Lista? Lista { get; set; }
        public  Dignidad? Dignidad { get; set; }
        public  OpcionConsulta? Opcion { get; set; }
        public  PreguntaConsulta? Pregunta { get; set; }
    }
}
