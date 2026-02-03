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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id{ get; set; }


        [Required, ForeignKey(nameof(TipoVoto))]
        public int IdTipoVoto { get; set; }


        [Required, ForeignKey(nameof(Junta))]
        public int IdJunta { get; set; }


        [Required, ForeignKey(nameof(Proceso))]
        public int IdProceso { get; set; }


        [Required, ForeignKey(nameof(Lista))]
        public int? IdLista { get; set; }


        [Required, ForeignKey(nameof(Dignidad))]
        public int? IdDignidad { get; set; }


        [Required, ForeignKey(nameof(Opcion))]
        public int? IdOpcion { get; set; }


        [Required, ForeignKey(nameof(Pregunta))]
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
