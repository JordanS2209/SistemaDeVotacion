using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class PreguntaConsulta
    {
        [Key] public int Id { get; set; }

        public string TextoPregunta { get; set; }

        public int NumeroPregunta { get; set; }


        [Required, ForeignKey("ProcesoElectoral")]
        public int IdProceso { get; set; }

        public  ProcesoElectoral? ProcesosElectorales { get; set; } = new ProcesoElectoral();
        public  List<OpcionConsulta>? OpcionesConsulta { get; set; } = new List<OpcionConsulta>();
        public  List<VotoDetalle>? VotosRecibidos { get; set; } = new List<VotoDetalle>();
    }
}
