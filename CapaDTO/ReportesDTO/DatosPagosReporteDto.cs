using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.ReportesDTO
{
    public class DatosPagosReporteDto
    {
        public int Id { get; set; }

        public string NombreBanco { get; set; }
        public string NumeroCuenta { get; set; }
        public string TipoCuenta { get; set; }
        public string CodigoSwift { get; set; }
        public string Ciudad { get; set; }
        public string Pais { get; set; }
        public string CorreoElectronico { get; set; }
        public int IdFormulario { get; set; }
        public string Sucursal { get; set; }
    }
}
