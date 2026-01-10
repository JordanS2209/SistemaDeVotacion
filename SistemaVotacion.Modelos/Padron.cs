using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Padron
    {
        [Key] public int Id { get; set; }

        public bool HaVotado { get; set; }

        public int IdProceso { get; set; }

        public int IdVotante { get; set; }


        public virtual ProcesoElectoral? Proceso { get; set; }

        public virtual Votante? Votante { get; set; }
    }
}
