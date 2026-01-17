using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Genero
    {
        [Key] public int IdGenero { get; set; }

        public string DetalleGenero { get; set; } 

        public  List<Usuario>? Usuarios { get; set; } = new List<Usuario>();
    }
}
