using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class VotoGeneralDto
    {
        [Required]
        public string CodigoAcceso { get; set; } = string.Empty;

        [Required]
        public int IdLista { get; set; }
    }
}
