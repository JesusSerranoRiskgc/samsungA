using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class ActividadDto
    {

        public int Id { get; set; }
        public string NombreActividad { get; set; }
        public string Descripcion { get; set; }

        public int IdArea { get; set; }


        public string NombreArea { get; set; }


        public bool Activo { get; set; }


    }
}
