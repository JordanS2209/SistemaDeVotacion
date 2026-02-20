using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Multimedia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UrlFoto { get; set; }

        public string? Descripcion { get; set; }


        [ForeignKey(nameof(Candidato))]
        public int? IdCandidato { get; set; }

        [ForeignKey(nameof(Lista))]
        public int? IdLista { get; set; }

        public  Candidato? Candidato { get; set; }

        public  Lista? Lista { get; set; }
    }
}
