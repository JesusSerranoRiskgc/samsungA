
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class InformacionComplementariaDto
    {
        public int Id { get; set; }
        public int IdFormulario { get; set; }
        public bool ActivosVirtuales { get; set; }
        public bool GrandesCantidadesEfectivo { get; set; }

        
        public bool InvestigadoViolacionLeyesAnticorrupcion { get; set; }
        public bool DeclaracionNoToleranciaCorrupcion { get; set; }
        public bool ExtensionColaboradoresPolitica { get; set; }
        public bool PoliticaAportesDonaciones { get; set; }
        public bool ContratadoTercerosOrganizacion { get; set; }

        public bool ObligadaSistemaPrevencionLAFT { get; set; }
        public bool TieneSistemaPrevencionLAFT { get; set; }
        public bool CasoRespuestaSistemaLAFT { get; set; }
        public bool AdopcionPoliticasLAFT { get; set; }
        public bool NombramientoOficialCumplimiento { get; set; }
        public bool MedidasDebidaDiligencia { get; set; }
        public bool IdentificacionEvaluacionRiesgos { get; set; }
        public bool IdentificacionReporteSospechosas { get; set; }
        public bool PoliticasCapacitacionLAFT { get; set; }


        public bool ObligadoAutocontrolLAFT { get; set; }
        public bool ObligadoProgramaPTEE { get; set; }
        public bool AdopcionPoliticasOrganoDireccion { get; set; }
        public bool EstablecimientoMedidasDebidaDiligencia { get; set; }
        public bool IdentificacionReportesOperSospechosas { get; set; }
        public bool RiesgosCorrupcionSobornoTransnacional { get; set; }
        public bool RiesgosLAFT { get; set; }
        public bool PoliticasCapacitacion { get; set; }
    }
}

