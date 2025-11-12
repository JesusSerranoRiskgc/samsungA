using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class DatoAduntoDto
    {
        public int Id { get; set; }
        public int IdFormulario { get; set; }

        public string NombreArchivo { get; set; }

        public string RutaArchivo { get; set; }
        public string TipoArchivo { get; set; }

        public string FechaSubida { get; set; }
        public string Telefono { get; set; }


    }
}
