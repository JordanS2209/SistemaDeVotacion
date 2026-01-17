using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Dignidad
    {
        [Key] public int Id { get; set; }

        public string NombreDignidad { get; set; }

        public  List<Candidato> Candidatos { get; set; } = new List<Candidato>();

        public  List<VotoDetalle> VotosRecibidos { get; set; } = new List<VotoDetalle>();
    }
}
