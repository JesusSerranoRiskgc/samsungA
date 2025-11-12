using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Respuestas
{
    public class FormularioDto
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public int IdEstado { get; set; }

        public string Oea { get; set; }
        public string Estado { get; set; }
        public string FechaFormulario { get; set; }
    }
}
