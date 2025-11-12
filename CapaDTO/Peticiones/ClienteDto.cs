using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class ClienteDto
    {
        public int Id { get; set; }
        public string Documento { get; set; }
        public string Nombre { get; set; }

        public bool Estado { get; set; }

        public int IdArea { get; set; }

        public string Area { get; set; }

    }
}
