using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class OpcionConsulta
    {
        [Key] public int Id { get; set; }

        public string TextoOpcion { get; set; }


        [Required, ForeignKey("PreguntaConsulta")]
        public int IdPregunta { get; set; }

        public  PreguntaConsulta? Pregunta { get; set; }

        public  List<VotoDetalle>? VotosRecibidos { get; set; }
    }
}
