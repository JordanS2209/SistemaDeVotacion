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
        
        public string NumeroJunta { get; set; }

        public int IdGenero { get; set; }

        public int IdRecinto { get; set; }

        public  Genero? Genero { get; set; }

        public  RecintoElectoral? Recintos { get; set; }

        public  List<Votante> VotantesAsignados { get; set; } = new List<Votante>();
        public  List<RepresentanteJunta> Representantes { get; set; } = new List<RepresentanteJunta>();
        public  List<VotoDetalle> VotosRecibidos { get; set; } = new List<VotoDetalle>();
        public  List<ActaAuditoria> ActasCierre { get; set; } = new List<ActaAuditoria>();
    }
}
