using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class DatosContactoDto
    {
        public int Id { get; set; }

        public string NombreContacto { get; set; }
        public string CargoContacto { get; set; }

        public string AreaContacto { get; set; }
        public string TelefonoContacto { get; set; }
        public string CorreoElectronico { get; set; }
        public int IdFormulario { get; set; }
        public string Ciudad { get; set; }
        public string Direccion { get; set; }



    }
}
