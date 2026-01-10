using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Candidato
    {
        [Key] public int Id { get; set; }
        public string NombreCandidato { get; set; }
        public int IdLista { get; set; }
        public int IdDignidad { get; set; }

        public  Lista? Lista { get; set; }

        public  Dignidad? Dignidad { get; set; }

        public virtual List<Multimedia> GaleriaMultimedia { get; set; } = new List<Multimedia>();
    }
}
