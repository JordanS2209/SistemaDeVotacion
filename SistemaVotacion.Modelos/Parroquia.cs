using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Parroquia
    {
        [Key] public int Id { get; set; }

        public string NombreParroquia { get; set; }

        public int CodigoPostalParr { get; set; }

        public int IdCiudad { get; set; }

        public  Ciudad? Ciudad { get; set; }

        public  List<RecintoElectoral>? Recintos { get; set; } = new List<RecintoElectoral>();
    }
}
