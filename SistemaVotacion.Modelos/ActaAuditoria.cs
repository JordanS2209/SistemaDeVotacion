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
        [Key] public int Id { get; set; }
        public int IdProceso { get; set; }
        public int IdJunta { get; set; }
        public int TotalSufragantesPadron { get; set; }
        public int VotosEnUrna { get; set; }
        public string HashSeguridad { get; set; } = null!;
        public DateTime FechaCierre { get; set; }

        public  ProcesoElectoral? Procesos { get; set; }
        public  JuntaReceptora? Juntas { get; set; }

        //public virtual List<ResultadoDetalleAuditoria> DetallesResultados { get; set; } = new List<ResultadoDetalleAuditoria>();
    }
}
