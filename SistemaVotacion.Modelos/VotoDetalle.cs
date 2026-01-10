using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class VotoDetalle
    {
        [Key] public int IdVoto { get; set; }
        public int IdTipoVoto { get; set; }
        public int IdJunta { get; set; }
        public int IdProceso { get; set; }

        public  TipoVoto? TipoVoto { get; set; }

        public  JuntaReceptora? Junta { get; set; }

        //public  ProcesoElectoral? Proceso { get; set; }
    }
}
