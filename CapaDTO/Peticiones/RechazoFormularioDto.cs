using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class RechazoFormularioDto
    {
        public int IdFormulario { get; set; }
        public string MotivoRechazo { get; set; } 
        public string FechaRechazo { get; set; }    
    }
}
