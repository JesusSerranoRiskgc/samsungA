using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class ReferenciaComercialesBancariasDtol
    {
        public int Id { get; set; }

        public int TipoReferencia { get; set; }
        public string NombreCompleto { get; set; }

        public string Ciudad { get; set; }
        public string Telefono { get; set; }
        public int IdFormulario { get; set; }
        public string ValorAnualCompras { get; set; }
        public string Cupo { get; set; }
        public string Plazo { get; set; }
    }
}
