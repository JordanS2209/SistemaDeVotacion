using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class TipoVoto
    {
        [Key] public int IdTipoVoto { get; set; }
        public string NombreTipo { get; set; } = null!;

        public  List<VotoDetalle>? VotosAsociados { get; set; } = new List<VotoDetalle>();
    }
}
