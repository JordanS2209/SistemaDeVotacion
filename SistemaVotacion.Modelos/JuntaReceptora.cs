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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public string NumeroJunta { get; set; }


        [Required, ForeignKey(nameof(Genero))]
        public int IdGenero { get; set; }


        [Required, ForeignKey(nameof(Recintos))]
        public int IdRecinto { get; set; }

        public  Genero? Genero { get; set; }

        public  RecintoElectoral? Recintos { get; set; }

        public  List<Votante>? VotantesAsignados { get; set; } 
        public  List<RepresentanteJunta>? Representantes { get; set; }
        public  List<VotoDetalle>? VotosRecibidos { get; set; } 
        public  List<ActaAuditoria>? ActasCierre { get; set; } 
    }
}
