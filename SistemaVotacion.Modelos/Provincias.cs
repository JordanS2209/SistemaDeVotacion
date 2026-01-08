using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Provincias
    {
        [Key] public int Id { get; set; }

        public string NombreProv { get; set; }

        public int CodigoPostal { get; set; }
    }
}
