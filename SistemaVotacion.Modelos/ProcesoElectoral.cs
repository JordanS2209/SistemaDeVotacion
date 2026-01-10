using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class ProcesoElectoral
    {
        [Key] public int Id { get; set; }

        public string NombreProceso { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public int IdTipoProceso { get; set; }

        public  TipoProceso? TipoProceso { get; set; }

        public  List<Lista>? Listas { get; set; } = new List<Lista>();

        public  List<Dignidad>? Dignidades { get; set; } = new List<Dignidad>();

        public  List<Padron>? RegistroPadron { get; set; } = new List<Padron>();
    }
}
