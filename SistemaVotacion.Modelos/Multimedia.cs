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
        [Key] public int Id { get; set; }

        public string UrlFoto { get; set; }

        public string? Descripcion { get; set; }


        [Required, ForeignKey("Candidato")]
        public int? IdCandidato { get; set; }


        [Required, ForeignKey("Lista")]
        public int? IdLista { get; set; }


        public  Candidato? Candidato { get; set; }

        public  Lista? Lista { get; set; }
    }
}
