using CapaDTO.Peticiones;
using CapaDTO.ReportesDTO;
using CapaDTO.Respuestas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos.Interfaz.Reporte.Interface
{
    public interface IReporteCapaDatos
    {
        Task<List<defaulchartsDto>> ReporteRegistroTiemposxarea(int IdUsuario);

        Task<List<defaulchartsDto>> ReporteRegistroTiemposxUsuario(int IdUsuario);

        Task<List<defaulchartsDto>> ReporteRegistroTiemposxCliente(int IdUsuario);


        Task<List<defaulchartsDto>> ReporteRegistroTiemposxServicio(int IdUsuario);


        Task<List<FormularioDto>> ContultaInfobasicaFormularioList(ReporteDto objetofiltro);


        Task<List<DatosGeneralesReporteDto>> ConsultaDatosGenerales(ReporteDto objetofiltro);

        Task<List<RepJunAccDTO>> ConsultaInfoRepresentanteslegales(ReporteDto objetofiltro);

        Task<List<RepJunAccDTO>> ConsultaInfoJuntaDirectivalegales(ReporteDto objetofiltro);

        Task<List<RepJunAccDTO>> ConsultaInfoAccionistas(ReporteDto objetofiltro);

        Task<List<DatosContactoDto>> ListaDatosContacto(ReporteDto objetofiltro);


        Task<List<ReferenciaComercialesBancariasReporteDto>> ListaReferenciasComercialesBan(ReporteDto objetofiltro);


        Task<List<DatosPagosReporteDto>> ConsultaDatosPago(ReporteDto objetofiltro);

        Task<List<DespachoMercanciaReporteDto>> ConsulataDespachoMercancia(ReporteDto objetofiltro);

        Task<List<CumplimientoNormativoDto>> ConsultaCumplimientoNormativo(ReporteDto objetofiltro);

        Task<List<ArchivoDto>> ConsultaInfoArchivoCargados(ReporteDto objetofiltro);

        Task<List<FormularioModelDTO>> ConsultaDatosInformacionOEA(ReporteDto objetofiltro);


        Task<List<InformacionTributariaDTO>> ConsultaInformacionTributaria(ReporteDto objetofiltro);



    }
}
