using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class ListasAdicionalesDto
    {
        public string TipoIdentificacion { get; set; }


        public int Tipo_Identificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string NombreCompleto { get; set; }

        public string IdFormulario { get; set; }
        public string NumeroConsulta { get; set; }
        public string FechaConsulta { get; set; }

    }
}
