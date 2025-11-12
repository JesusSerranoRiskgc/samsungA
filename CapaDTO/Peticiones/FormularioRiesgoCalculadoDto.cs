using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class FormularioRiesgoCalculadoDto
    {
        public string CategoriaTerceroRiesgo { get; set; }
        public string PaisRiesgoCorrupcion { get; set; }
        public string PaisRiesgoGAFI { get; set; }
        public string EsPEP_SiNoRiesgo { get; set; }
        public string CotizaEnBolsa_SiNoRiesgo { get; set; }
        public string ManejaEfectivo_SiNoRiesgo { get; set; }
        public string ManejaActivosVirtuales_SiNoRiesgo { get; set; }
        public string TamanoSociedadRiesgo { get; set; }

        public int Id { get; set; }
        public int IdFormulario { get; set; }
        public int ValorPersona { get; set; }
        public int ValorPaisCorrupcion { get; set; }
        public int ValorPaisGAFI { get; set; }
        public int ValorPEP { get; set; }
        public int ValorCotizaBolsa { get; set; }
        public int ValorTamano { get; set; }
        public int ValorOperacionesEfectivo { get; set; }
        public int ValorActivosVirtuales { get; set; }
        public int TotalRiesgo { get; set; }
        public string NivelRiesgoFinal { get; set; }
        public DateTime FechaCalculo { get; set; }

        // Campos descriptivos adicionales
        public string NombrePais { get; set; }
        public string CategoriaTercero { get; set; }
        public string TamanoTercero { get; set; }
        public string RazonPaisCorrupcion { get; set; }
        public string RazonPaisGAFI { get; set; }
        public bool ManejaEfectivo { get; set; }
        public bool ManejaActivosVirtuales { get; set; }
        public bool EsPEP { get; set; }
        public bool CotizaEnBolsa { get; set; }
    }
}
