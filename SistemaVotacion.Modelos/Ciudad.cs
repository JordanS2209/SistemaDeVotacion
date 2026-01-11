using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Ciudad
    {
        [Key] public int Id { get; set; }
        
        [Required, MaxLength(100)]
        public string NombreCiu{ get; set; }

        [Required, ForeignKey("Provincia")]
        public int IdProvincia { get; set; }

        public  Provincia? Provincia { get; set; }

        public List <Parroquia>? Parroquias { get; set; } 

    }
}
