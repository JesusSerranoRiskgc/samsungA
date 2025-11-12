using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class  FormularioModelDTO
    {
        public int IdFormulario { get; set; }
        public string Uen { get; set; }
        public string ResponsableVenta { get; set; }
        public string CorreoElectronico { get; set; }
        public string ResponsableCartera { get; set; }
        public string ResponsableTecnico { get; set; }
        public string Moneda { get; set; }
        public string FormaPago { get; set; }
        public int NumeroDias { get; set; }
        public string CadenaLogistica { get; set; }
        public string ListasRiesgo { get; set; }
        public string SustanciasNarcoticos { get; set; }
        public string Certificaciones { get; set; }
        public string ProveedorCadenaLogistica { get; set; }
        public string RiesgoPais { get; set; }
        public string AntiguedadEmpresa { get; set; }
        public string RiesgoSeguridad { get; set; }
        public string Valoracion { get; set; }
        public string ListasRiesgoCliente { get; set; }
        public string TipoNegociacion { get; set; }
        public string VistoBuenoAseguradora { get; set; }
        public string RiesgoPaisCliente { get; set; }
        public string CertificacionesInstitucionalidad { get; set; }
        public string RiesgoSeguridadCliente { get; set; }
        public string ValoracionCliente { get; set; }
        public string SegmentacionRiesgo { get; set; } // Opcional
    }
}
