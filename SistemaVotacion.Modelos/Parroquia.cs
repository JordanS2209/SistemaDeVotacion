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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string NombreParroquia { get; set; }

        [Required, ForeignKey(nameof(Ciudad))]
        public int IdCiudad { get; set; }

        public  Ciudad? Ciudad { get; set; }

        public  List<RecintoElectoral>? Recintos { get; set; }
    }
}
