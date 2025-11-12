using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class DatosRevisorFiscalDto
    {
        public int Id { get; set; }
        public int IdFormulario { get; set; }
        public bool TieneRevisorFiscal { get; set; }
        public string JustificarRespuesta { get; set; }
        public bool RevisorFiscalAdscritoFirma { get; set; }
        public string NombreFirma { get; set; }
        public string NombreCompletoApellidos { get; set; }
        public string TipoID { get; set; }
        public string NumeroID { get; set; }
        public string Telefono { get; set; }
        public string Ciudad { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
    }
}
