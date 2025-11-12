using CapaDTO.Peticiones;
using CapaDTO.Respuestas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Interfaz.RegistroFormulario.Interface
{
    public interface IRegistroFormularioCapaNegocio
    {
        Task<FormularioDto> CrearNuenoFormulario(int IdUsuario);

        Task<FormularioDto> ReplicaFormulario(int IdFormularioAnterior, int IdUsuario);

        Task<List<FormularioDto>> ListaFormularios();

        Task<List<FormularioDto>> ListaFormulariosContabilidad();

        Task<List<FormularioDto>> ListaFormulariosControlInterno();
        Task<List<FormularioDto>> ListaFormulariosCompradorVendedor(int IdUsuario);

        Task<List<FormularioDto>> ListaFormulariosOficialCumplimiento();

        Task<bool> GuardarDatosGenerales(DatosGeneralesDto objRegistro);

        Task<bool> GuardarDatosGenerales(FormularioModelDTO objRegistro, int IdUsuario);
        Task<bool> GuardaInformacionContactos(List<DatosContactoDto> objRegistro);

        Task<bool> GuardaReferenciaComercialBanc(List<ReferenciaComercialesBancariasDtol> objRegistro);

        Task<bool> GuardaInformacionComplementaria(InformacionComplementariaDto objRegistro);
        Task<bool> GuardaInformacionFinanciera(InformacionFinancieraDto objRegistro);

        Task<bool> GuardaDatosRevisorFiscal(DatosRevisorFiscalDto objRegistro);
        Task<bool> GuardaDatosPago(DatosPagosDto objRegistro);

        Task<bool> GuardaCumplimientoNormativo(CumplimientoNormativoDto objRegistro);

        Task<bool> GuardaDespachoMercancia(DespachoMercanciaDto objRegistro);
        Task<DatosGeneralesDto> ConsultaDatosGenerales(int IdFormulario);

        Task<List<DatosContactoDto>> ListaDatosContacto(int IdFormulario);

        Task<List<ReferenciaComercialesBancariasDtol>> ListaReferenciasComercialesBan(int IdFormulario);

        Task<DatosPagosDto> ConsultaDatosPago(int IdFormulario);

        Task<CumplimientoNormativoDto> ConsultaCumplimientoNormativo(int IdFormulario);

        Task<DespachoMercanciaDto> ConsulataDespachoMercancia(int IdFormulario);

        Task<InformacionComplementariaDto> ConsultaInformacionComplementaria(int IdFormulario);

        Task<InformacionFinancieraDto> ConsultaInformacionFinanciera(int IdFormulario);

        Task<DatosRevisorFiscalDto> ConsultaDatosRevisorFiscal(int IdFormulario);

        Task<bool> GuardaInformacionRepresentantesLegales(int IdFormulario, object objrepresetantes);

        Task<object> ConsultaInfoRepresentanteslegales(int IdFormulario);

        Task<bool> GuardaInformacionAccionistas(int IdFormulario, object objAccionista);

        Task<object> ConsultaInfoAccionistas(int IdFormulario);


        Task<bool> GuardaInformacionJuntaDirectiva(int IdFormulario, object objjuntadirectiva);

        Task<object> ConsultaInfoJuntaDirectiva(int IdFormulario);

        Task<bool> CambiaEstadoFormulario(int IdFormulario, int IdEstado);

        Task<bool> CalcularRiesgoFormulario(int IdFormulario);

        Task<FormularioRiesgoCalculadoDto> ObtenerRiesgoFormulario(int IdFormulario);

        Task<List<FormularioDto>> ListaFormulariosbyClienteProveedor(int IdUsuario,string Lang);


        Task<bool> GuardaInfoAdjuntos(ArchivoDto objAdjunto);

        Task<ArchivoDto> ConsultaInfoArchivo(int IdFormualrio, string Key);

        Task<bool> EliminaArchivoBasedatos(int idArchivo);

        Task<List<ArchivoDto>> ConsultaInfoArchivoCargados(int IdFormualrio);

        Task<bool> GuardaMotivoRechazoFormulario(RechazoFormularioDto objRechazo, int IdEstado, int IdUsuario);

        Task<RechazoFormularioDto> MuestraMotivoRechazo(int IdFormulario);

        Task<List<InformacionInspektorDto>> ListaResultadosInspektor(int IdFormulario);

        Task EnviarNotificacionClienteEnviaForm(int IdFormulario, string path);

        Task EnviarNotificacionPendienteCorrecciones(int IdFormulario, RechazoFormularioDto objRechazo);

        Task<FormularioDto> InfoFormulario(int IdFormualrio);

        Task NotificacionFormularioCorregidoClienteForm(int IdFormulario, string path);

        Task EnviarNotificacionRechazadoporControlInterno(int IdFormulario, RechazoFormularioDto objRechazo);


        Task<List<FormularioDto>> ListaFormulariosUsuarioOEA();

        Task<bool> GuardarInformacionOEA(FormularioModelDTO objRegistro, int IdUsuario);

        Task<FormularioModelDTO> ConsultaDatosInformacionOEA(int IdFormulario);

        Task<bool> ApruebaFormulario(int IdFormulario, int Estado);

        Task EnviarNotificacionTerceroRechazadoOficialCumplimineto(int IdFormulario, RechazoFormularioDto objRechazo);

        Task<bool> GuardarDeclaraciones(DeclaracionesDto objRegistro);

        Task<DeclaracionesDto> ConsultaDeclaraciones(int IdFormulario);


        Task<bool> GuardarInformacionTriburaria(InformacionTributariaDTO objRegistro);

        Task<InformacionTributariaDTO> ConsultaInformacionTributaria(int IdFormulario);

        Task GuardaInfoServiciosAdicionales(int IdFormulario, string NumeroConsulta, object jsonProcuraduria, object jsonRamaJudicial, object ejecucionPenas);
        Task<MemoryStream> ConsultaRamaJudicial(ListasAdicionalesDto objetofiltro);

        Task<MemoryStream> ConsultaInfoProcuraduria(ListasAdicionalesDto objetofiltro);

        Task<MemoryStream> EjecucucionPenas(ListasAdicionalesDto objetofiltro);

        Task ValidaPaisesRiesgo(int IdFormulario);

        Task<bool> GuardaConflictoInteres(ConflictoInteresDto objRegistro);
        Task<ConflictoInteresDto> ConsultaConflictoInteres(int IdFormulario);

        Task<string> ObtieneTockenConsultaInspektor();

    }
}
