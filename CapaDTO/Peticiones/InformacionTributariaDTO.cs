using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class InformacionTributariaDTO
    {
        public int Id { get; set; }
        public int IdFormulario { get; set; }
        public int GranContribuyente { get; set; }
        public string NumResolucionGranContribuyente { get; set; }
        public string FechaResolucionGranContribuyente { get; set; }
        public int Autorretenedor { get; set; }
        public string NumResolucionAutorretenedor { get; set; }
        public string FechaResolucionAutorretenedor { get; set; }
        public int ResponsableICA { get; set; }
        public string MunicipioRetener { get; set; }
        public int Tarifa { get; set; }
        public int ResponsableIVA { get; set; }
        public int AgenteRetenedorIVA { get; set; }
        public string Sucursal { get; set; }
        public string RegimenTributario { get; set; }
        public object Retenciones { get; set; }

    }


}
