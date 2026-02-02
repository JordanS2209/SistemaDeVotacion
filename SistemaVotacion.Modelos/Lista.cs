using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Lista
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string NombreLista { get; set; }
        public int NumeroLista { get; set; }

        [Required]
        public int IdProceso { get; set; }

        [ForeignKey(nameof(IdProceso))]
        public ProcesoElectoral? Proceso { get; set; }

        public List<Candidato>? Candidatos { get; set; }
        public List<Multimedia>? RecursosMultimedia { get; set; }
        public List<VotoDetalle>? VotosRecibidos { get; set; }
    }
}
