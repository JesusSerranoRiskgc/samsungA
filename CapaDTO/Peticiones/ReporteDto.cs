using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class ReporteDto
    {
        public int IdUsuario { get; set; }
        public int IdEstado { get; set; }
        public string FechaDe { get; set; }
        public string FechaHasta { get; set; }

    }
}
