using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.ERP
{
    public class PeticionRespuestaERPDTO
    {
        public int Id { get; set; }
        public int IdFormulario { get; set; }
        public object Peticion  { get; set; }
        public object Respuesta { get; set; }

    }
}
