using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Votante
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required,ForeignKey(nameof(Junta))]
        public int IdJunta { get; set; }

         [Required, ForeignKey(nameof(Usuario))]
        public int IdUsuario { get; set; }
        public bool Estado { get; set; } = false;

        public  Usuario? Usuario { get; set; }

        public  JuntaReceptora? Junta { get; set; }

        public List<Padron>? VotantesEnPadron { get; set; }
    }
}
