using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class JuntaReceptora
    {
        [Key] public int Id { get; set; }
        
        [Required, MaxLength(10)]
        public string NumeroJunta { get; set; }
        public string? Genero { get; set; }

        [Required, ForeignKey("RecintoElectoral")]
        public int IdRecinto { get; set; }

        public  RecintoElectoral? Recintos { get; set; }

        public  List<Votante>? Votantes { get; set; } 

        public  List<VotoDetalle>? Votos { get; set; }
    }
}
