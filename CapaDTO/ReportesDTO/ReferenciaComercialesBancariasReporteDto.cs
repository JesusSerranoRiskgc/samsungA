using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.ReportesDTO
{
    public class ReferenciaComercialesBancariasReporteDto
    {
        public int Id { get; set; }

        public string TipoReferencia { get; set; }
        public string NombreCompleto { get; set; }
        public string Ciudad { get; set; }
        public string Telefono { get; set; }
        public int IdFormulario { get; set; }
    }
}
