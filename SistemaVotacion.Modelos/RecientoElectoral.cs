using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class RecientoElectoral
    {
        [Key] public int Id { get; set; }

        public string NombreRecinto { get; set; }

        public string DetalleRecinto { get; set; }

        public string? DireccionRecinto { get; set; }

        public int IdParroquia { get; set; }

        public Parroquia? Parroquia { get; set; }
    }
}
