using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Respuestas
{
    public class ProcesosRamaJudicialDto
    {
        public int? idProceso { get; set; }

        public string llaveProceso { get; set; }
        public string fechaProceso { get; set; }

        public string fechaUltimaActuacion { get; set; }

        public string despacho { get; set; }      

        public string departamento { get; set; }

        public string sujetosProcesales { get; set; }
    }
}
