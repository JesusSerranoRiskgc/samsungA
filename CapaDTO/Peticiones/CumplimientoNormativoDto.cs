using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class CumplimientoNormativoDto
    {
        public int Id { get; set; }
        public int IdFormulario { get; set; }
        public bool sometida_sagrilaft { get; set; }
        public bool sometida_otro_sistema { get; set; }
        public bool adhesion_politicas_samsung { get; set; }
        public bool no_invest_sancion_laftfpadm { get; set; }
        public bool no_transacciones_ilicitas { get; set; }
        public bool acepta_monitoreo_info { get; set; }
        public bool no_listas_restrictivas { get; set; }
        public string correo_reportar_incidentes { get; set; }
    }
}
