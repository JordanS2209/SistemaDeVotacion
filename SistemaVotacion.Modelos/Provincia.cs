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

        [Required, MaxLength(100)]
        public string NombreProv{ get; set; }

        public List<Ciudad>? Ciudades { get; set; } 

    }
}
