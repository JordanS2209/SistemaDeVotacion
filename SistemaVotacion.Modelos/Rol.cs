using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVotacion.Modelos
{
    public class Rol
    {
        [Key] public int Id { get; set; }
        
        [Required, MaxLength(50)]
        public string NombreRol { get; set; }
        
        [Required, MaxLength(100)]
        public string DescripcionRol { get; set; }
        public List<Usuario>? Usuarios { get; set; }
    }
}
