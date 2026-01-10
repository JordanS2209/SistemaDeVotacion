using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Boleta
    {
        [Key]
        public int IdBoleta { get; set; }

        // Proceso electoral al que pertenece
        public int IdProcesoElectoral { get; set; }
        public ProcesoElectoral ProcesoElectoral { get; set; }

        // Dignidad que se vota (Presidente, Alcalde, etc.)
        public int IdDignidad { get; set; }
        public Dignidad Dignidad { get; set; }

        // Lista seleccionada
        public int IdLista { get; set; }
        public Lista Lista { get; set; }

        // Tipo de voto
        public int IdTipoVoto { get; set; }
        public TipoVoto TipoVoto { get; set; }

        // Control
        public DateTime FechaEmision { get; set; }
        public bool Emitida { get; set; }
    }
}
