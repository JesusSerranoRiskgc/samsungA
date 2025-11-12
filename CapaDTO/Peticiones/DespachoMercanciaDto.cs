using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class DespachoMercanciaDto
    {
        public int Id { get; set; }
        public int IdFormulario { get; set; }

        public string DireccionDespacho { get; set; }

        public int Pais { get; set; }
        public string Cuidad { get; set; }

        public string CodigoPostalEnvio { get; set; }
        public string Telefono { get; set; }

        public string EmailCorporativo { get; set; }
    }
}
