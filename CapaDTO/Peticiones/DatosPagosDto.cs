using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class DatosPagosDto
    {
        public int Id { get; set; }

        public string NombreBanco { get; set; }
        public string NumeroCuenta { get; set; }
        public int TipoCuenta { get; set; }
        public string CodigoSwift { get; set; }
        public string Ciudad { get; set; }
        public int Pais { get; set; }
        public string CorreoElectronico { get; set; }
        public int IdFormulario { get; set; }
        public string Sucursal { get; set; }
    }
}
