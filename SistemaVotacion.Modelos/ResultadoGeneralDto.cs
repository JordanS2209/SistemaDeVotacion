using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class ResultadoGeneralDto
    {
        public string Lista { get; set; }
        public int Numero { get; set; }
        public int Votos { get; set; }
        public string UrlLogo { get; set; }
        public bool EsValido { get; set; }
    }
}
