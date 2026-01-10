using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Lista
    {
        [Key] public int IdLista { get; set; }
        public string NombreLista { get; set; } 
        public int NumeroLista { get; set; }
        public int IdProceso { get; set; }

         public  ProcesoElectoral? Procesos { get; set; }

        //public  List<Candidato> Candidatos { get; set; } = new List<Candidato>();

        //public  List<Multimedia> RecursosMultimedia { get; set; } = new List<Multimedia>();
    }
}
