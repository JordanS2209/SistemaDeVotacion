using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Representantes
    {
        [Key] public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdJunta { get; set; }
        public int IdRol { get; set; }
        public int IdProceso { get; set; }

        public  Usuario? Usuario { get; set; }
        public  JuntaReceptora? Junta { get; set; }
        public  Rol? Rol { get; set; }
        public  ProcesoElectoral? Proceso { get; set; }
    }
}
