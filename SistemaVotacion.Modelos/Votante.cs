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
        [Key] public int Id { get; set; }

        public int IdJunta { get; set; }

        public int IdUsuario { get; set; }

        public virtual Usuario? Usuario { get; set; }

        //public  JuntaReceptora? Junta { get; set; }

        //public List<Padron> ParticipacionesEnPadron { get; set; } = new List<Padron>();
    }
}
