using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Provincia
    {
        [Key] public int Id { get; set; }

        public string NombreProvincia{ get; set; }

        public List<Ciudad>? Ciudades { get; set; } 

    }
}
