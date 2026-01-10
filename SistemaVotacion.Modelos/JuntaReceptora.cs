using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class JuntaReceptora
    {
        [Key] public int IdJunta { get; set; }
        public string NumeroJunta { get; set; }
        public string? Genero { get; set; }
        public int IdRecinto { get; set; }

        public  RecintoElectoral? Recintos { get; set; }

        public  List<Votante>? Votantes { get; set; } = new List<Votante>();

        public  List<VotoDetalle>? Votos { get; set; } = new List<VotoDetalle>();
    }
}
