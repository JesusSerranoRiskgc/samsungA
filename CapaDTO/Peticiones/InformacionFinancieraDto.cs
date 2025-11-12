using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class InformacionFinancieraDto
    {
        public int Id { get; set; }
        public int IdFormulario { get; set; }
        public decimal Patrimonio { get; set; }
        public decimal Activos { get; set; }
        public decimal IngresosMensuales { get; set; }
        public decimal EgresosMensuales { get; set; }
        public bool ActivosVirtuales { get; set; }
        public bool GrandesCantidadesEfectivo { get; set; }


    }
}

