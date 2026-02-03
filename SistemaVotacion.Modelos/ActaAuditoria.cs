using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class ActaAuditoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }


        [Required, ForeignKey(nameof(Procesos))]
        public int IdProceso { get; set; }

        [Required, ForeignKey(nameof(Juntas))]
        public int IdJunta { get; set; }

        public int TotalSufragantesPadron { get; set; }

        public int VotosEnUrna { get; set; }

        public string HashSeguridad { get; set; }

        public DateTime FechaCierre { get; set; }

        public string? Observaciones { get; set; }

        public  ProcesoElectoral? Procesos { get; set; }

        public  JuntaReceptora? Juntas { get; set; }

        public  List<ResultadoDetalleAuditoria>? DetallesResultados { get; set; }
    }
}
