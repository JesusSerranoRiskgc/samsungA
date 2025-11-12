using CapaDTO.Peticiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Respuestas
{
    public class CurrentUserInfo
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Usuario { get; set; }
        public string Rol { get; set; }


        public bool ActualizarPass { get; set; }


    }
}
