using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class ResultadoDetalleAuditoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required, ForeignKey("ActaAuditoria")]
        public int IdActa { get; set; }


        [Required, ForeignKey("Lista")]
        public int? IdLista { get; set; }

        public int VotosContabilizados { get; set; }

        public  ActaAuditoria? Acta { get; set; }

        public  Lista? Lista { get; set; }
    }
}
