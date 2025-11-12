using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class PeticionInspektorDTO
    {
        public string nombre { get; set; }

        public int tipoDocumento { get; set; }
        public string identificacion { get; set; }
        public string cantidadPalabras { get; set; }
        public bool tienePrioridad_4 { get; set; }

        public bool procuraduria { get; set; }
        public bool ramaJudicial { get; set; }
        public bool ramaJEPMS { get; set; }
    }
}
