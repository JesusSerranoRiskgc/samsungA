using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class DeclaracionesDto
    {
        public int Id { get; set; }
        public int IdFormulario { get; set; }
        public string NombreRepresentanteFirma { get; set; }
        public string CorreoRepresentante { get; set; }
    }
}
