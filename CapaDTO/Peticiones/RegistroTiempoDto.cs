using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class RegistroTiempoDto
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public int IdArea { get; set; }
        public string NombreArea { get; set; }

        public int IdActividad { get; set; }
        public string NombreActividad { get; set; }

        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }

        public int IdServicio { get; set; }
        public string NombreServicio { get; set; }

        public decimal NumeroHoras { get; set; }
        public string FechaActividad { get; set; }

        public string Observacion { get; set; }

    }
}
