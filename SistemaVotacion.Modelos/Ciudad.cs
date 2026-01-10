using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Ciudad
    {
        [Key] public int Id { get; set; }

        public string NombreCiu { get; set; }

        public int CodigoPostalCiudad { get; set; }

        public int IdProvincia { get; set; }

        public  Provincia? Provincia { get; set; }

        public List <Parroquia>? Parroquias { get; set; } = new List<Parroquia>();

    }
}
