using CapaDatos.Interfaz.RegistroFormulario.Interface;
using CapaDatos.util;
using CapaDTO.ERP;
using CapaDTO.Peticiones;
using CapaDTO.ReportesDTO;
using CapaDTO.Respuestas;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CapaDatos.Implementacion.RegistroFormulario.Implementacion
{
    public class clsRegistroFormularioCapaDatos : IRegistroFormularioCapaDatos
    {


        private cDataBase cDataBase;
        private readonly IConfiguration _configuration;

        public clsRegistroFormularioCapaDatos(IConfiguration configuration)
        {
            _configuration = configuration;
            cDataBase = new cDataBase(_configuration);
        }



        public async Task<FormularioDto> CrearNuenoFormulario(int IdUsuario)
        {
            string strConsulta = string.Empty;

            FormularioDto NuevoFormulario = new FormularioDto();


            DataTable dtInformacion = new DataTable();

            DateTime FechaCreacion = DateTime.Now;

            string fechaFormateada = FechaCreacion.ToString("dd-MM-yyyy HH:mm");

            try
            {
                int scope = 0;
                DateTime fecha = DateTime.Parse(fechaFormateada);
                string fechaSQL = fecha.ToString("yyyy-MM-dd HH:mm:ss"); // formato seguro
                strConsulta = string.Format(
                  "INSERT INTO [dbo].[FormularioClienteProveedores] VALUES ({0},1,'{1}') SELECT SCOPE_IDENTITY()",
                  IdUsuario, fechaSQL);
                cDataBase.conectar();
                dtInformacion = cDataBase.mtdEjecutarConsultaSQL(strConsulta);
                cDataBase.desconectar();

                if (dtInformacion.Rows.Count > 0)
                {
                    scope = Convert.ToInt32(dtInformacion.Rows[0][0]);

                    NuevoFormulario.Id = scope;
                    NuevoFormulario.IdUsuario = IdUsuario;
                    NuevoFormulario.NombreUsuario = "";
                    NuevoFormulario.IdEstado = 1;
                    NuevoFormulario.Estado = "Creado / Created";
                    NuevoFormulario.FechaFormulario = fechaFormateada;
                }
                else
                {
                    throw new InvalidOperationException("error al crear el Cliente");
                    NuevoFormulario = null;
                }



            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");

            }

            return NuevoFormulario;
        }


        public async Task<FormularioDto> ReplicaFormulario(int IdFormularioAnterior, int IdUsuario)
        {
            string strConsulta = string.Empty;

            FormularioDto NuevoFormulario = new FormularioDto();


            DataTable dtInformacion = new DataTable();

            DateTime FechaCreacion = DateTime.Now;

            string fechaFormateada = FechaCreacion.ToString("dd-MM-yyyy HH:mm");

            try
            {
                int scope = 0;
                strConsulta = string.Format("Insert into [dbo].[FormularioClienteProveedores] values ({0},1,'{1}') SELECT SCOPE_IDENTITY()", IdUsuario, fechaFormateada);

                cDataBase.conectar();
                dtInformacion = cDataBase.mtdEjecutarConsultaSQL(strConsulta);
                cDataBase.desconectar();

                if (dtInformacion.Rows.Count > 0)
                {
                    scope = Convert.ToInt32(dtInformacion.Rows[0][0]);

                    NuevoFormulario.Id = scope;
                    NuevoFormulario.IdUsuario = IdUsuario;
                    NuevoFormulario.NombreUsuario = "";
                    NuevoFormulario.IdEstado = 1;
                    NuevoFormulario.Estado = "Creado / Created";
                    NuevoFormulario.FechaFormulario = fechaFormateada;

                    bool copoatablas = await CopiaTablas(scope, IdFormularioAnterior);

                }
                else
                {
                    throw new InvalidOperationException("error al crear el Cliente");
                    NuevoFormulario = null;
                }
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");

            }

            return NuevoFormulario;
        }

        public async Task<bool> CopiaTablas(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            bool DatosGenerales = await CopiaDatosGenerarles(IdFormularioNuevo, IdFormularioAnterior);

            bool representantes = await CopiaRepresentantesLegales(IdFormularioNuevo, IdFormularioAnterior);



            bool juntadirectiva = await CopiaJuntadirectiva(IdFormularioNuevo, IdFormularioAnterior);


            bool accionistas = await CopiaAccionistas(IdFormularioNuevo, IdFormularioAnterior);



            bool datoscontacto = await CopiaDatosContacto(IdFormularioNuevo, IdFormularioAnterior);
            bool datospago = await CopiaDatosPgo(IdFormularioNuevo, IdFormularioAnterior);
            bool despachomercancia = await CopiaDespachoMercancia(IdFormularioNuevo, IdFormularioAnterior);

            bool cumplimiento = await CopiaCumplimientoNormativo(IdFormularioNuevo, IdFormularioAnterior);


            bool tributaria = await CopiaTibutaria(IdFormularioNuevo, IdFormularioAnterior);

            bool referencias = await CopiaReferenciasComerciales(IdFormularioNuevo, IdFormularioAnterior);



            bool declaraciones = await CopiaDeclaraciones(IdFormularioNuevo, IdFormularioAnterior);




            return true;
        }



        public async Task<bool> CopiaDatosGenerarles(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            DatosGeneralesDto datosgenerarles = new DatosGeneralesDto();
            datosgenerarles = await ConsultaDatosGenerales(IdFormularioAnterior);

            datosgenerarles.IdFormulario = IdFormularioNuevo;
            datosgenerarles.Id = 0;
            return await GuardarDatosGenerales(datosgenerarles);

        }


        public async Task<bool> CopiaRepresentantesLegales(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            object representantes = await ConsultaInfoRepresentanteslegales(IdFormularioAnterior);

            if (representantes != null)
            {
                return await GuardaInformacionRepresentantesLegales(IdFormularioNuevo, representantes);
            }

            return true;
        }

        public async Task<bool> CopiaJuntadirectiva(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            object representantes = await ConsultaInfoJuntaDirectiva(IdFormularioAnterior);

            if (representantes != null)
            {
                return await GuardaInformacionJuntaDirectiva(IdFormularioNuevo, representantes);
            }

            return true;
        }

        public async Task<bool> CopiaAccionistas(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            object representantes = await ConsultaInfoAccionistas(IdFormularioAnterior);

            if (representantes != null)
            {
                return await GuardaInformacionAccionistas(IdFormularioNuevo, representantes);
            }

            return true;
        }


        public async Task<bool> CopiaCumplimientoNormativo(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            CumplimientoNormativoDto cumplimiento = new CumplimientoNormativoDto();
            cumplimiento = await ConsultaCumplimientoNormativo(IdFormularioAnterior);

            if (cumplimiento != null)
            {
                cumplimiento.Id = 0;
                cumplimiento.IdFormulario = IdFormularioNuevo;


                return await GuardaCumplimientoNormativo(cumplimiento);
            }

            return true;
        }


        public async Task<bool> CopiaDatosContacto(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            List<DatosContactoDto> lstContactos = new List<DatosContactoDto>();
            lstContactos = await ListaDatosContacto(IdFormularioAnterior);

            if (lstContactos != null)
            {
                foreach (DatosContactoDto contacto in lstContactos)
                {
                    contacto.Id = 0;
                    contacto.IdFormulario = IdFormularioNuevo;
                }
                return await GuardaInformacionContactos(lstContactos);
            }

            return true;
        }

        public async Task<bool> CopiaDatosPgo(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            DatosPagosDto pago = new DatosPagosDto();
            pago = await ConsultaDatosPago(IdFormularioAnterior);

            if (pago != null)
            {
                pago.Id = 0;
                pago.IdFormulario = IdFormularioNuevo;


                return await GuardaDatosPago(pago);
            }

            return true;
        }

        public async Task<bool> CopiaDespachoMercancia(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            DespachoMercanciaDto desoachos = new DespachoMercanciaDto();
            desoachos = await ConsulataDespachoMercancia(IdFormularioAnterior);

            if (desoachos != null)
            {
                desoachos.Id = 0;
                desoachos.IdFormulario = IdFormularioNuevo;


                return await GuardaDespachoMercancia(desoachos);
            }

            return true;
        }


        public async Task<bool> CopiaTibutaria(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            InformacionTributariaDTO tributaria = new InformacionTributariaDTO();
            tributaria = await ConsultaInformacionTributaria(IdFormularioAnterior);

            if (tributaria != null)
            {
                tributaria.Id = 0;
                tributaria.IdFormulario = IdFormularioNuevo;


                return await GuardarInformacionTriburaria(tributaria);
            }

            return true;
        }



        public async Task<bool> CopiaReferenciasComerciales(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            List<ReferenciaComercialesBancariasDtol> lstReferencia = new List<ReferenciaComercialesBancariasDtol>();
            lstReferencia = await ListaReferenciasComercialesBan(IdFormularioAnterior);

            if (lstReferencia != null)
            {
                foreach (ReferenciaComercialesBancariasDtol contacto in lstReferencia)
                {
                    contacto.Id = 0;
                    contacto.IdFormulario = IdFormularioNuevo;
                }
                return await GuardaReferenciaComercialBanc(lstReferencia);
            }

            return true;
        }

        public async Task<bool> CopiaDeclaraciones(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            DeclaracionesDto declaraciones = new DeclaracionesDto();
            declaraciones = await ConsultaDeclaraciones(IdFormularioAnterior);

            if (declaraciones != null)
            {
                declaraciones.Id = 0;
                declaraciones.IdFormulario = IdFormularioNuevo;


                return await GuardarDeclaraciones(declaraciones);
            }

            return true;
        }





        public async Task<bool> CambiaEstadoFormulario(int IdFormulario, int IdEstado)
        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[FormularioClienteProveedores] " +
               "SET [IdEstado] = @IdEstado " +
               "WHERE [Id] = @Id";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                    new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                   new SqlParameter() { ParameterName = "@IdEstado", SqlDbType = SqlDbType.Int, Value =  IdEstado },

                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar");
            }
            return respuesta;

        }


        public async Task<List<FormularioDto>> ListaFormulariosbyUser(int IdUsuario)
        {

            return null;

        }

        public async Task<List<FormularioDto>> ListaFormularios()
        {

            List<FormularioDto> ListaFormularios = new List<FormularioDto>();
            DataTable dtInformacion = ConsultaListaFormulario();

            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    FormularioDto objForm = new FormularioDto();
                    objForm.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objForm.IdUsuario = Convert.ToInt32(dtInformacion.Rows[rows]["IdUsuario"]);
                    objForm.NombreUsuario = dtInformacion.Rows[rows]["NombreUsuario"].ToString();
                    objForm.IdEstado = Convert.ToInt32(dtInformacion.Rows[rows]["IdEstado"]);
                    objForm.Estado = dtInformacion.Rows[rows]["Estado"].ToString();
                    objForm.Oea = dtInformacion.Rows[rows]["ExisteInformacionOEA"].ToString();
                    objForm.FechaFormulario = dtInformacion.Rows[rows]["FechaFormulario"].ToString();
                    ListaFormularios.Add(objForm);
                }
            }

            return ListaFormularios;
        }

        public async Task<List<FormularioDto>> ListaFormulariosCompradorVendedor(int IdUsuario)
        {

            List<FormularioDto> ListaFormularios = new List<FormularioDto>();
            DataTable dtInformacion = ConsultaListaFormularioCompradorVendedor(IdUsuario);

            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    FormularioDto objForm = new FormularioDto();
                    objForm.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objForm.IdUsuario = Convert.ToInt32(dtInformacion.Rows[rows]["IdUsuario"]);
                    objForm.NombreUsuario = dtInformacion.Rows[rows]["NombreUsuario"].ToString();
                    objForm.IdEstado = Convert.ToInt32(dtInformacion.Rows[rows]["IdEstado"]);
                    objForm.Estado = dtInformacion.Rows[rows]["Estado"].ToString();
                    objForm.Oea = dtInformacion.Rows[rows]["ExisteInformacionOEA"].ToString();
                    objForm.FechaFormulario = dtInformacion.Rows[rows]["FechaFormulario"].ToString();
                    ListaFormularios.Add(objForm);
                }
            }

            return ListaFormularios;
        }

        private DataTable ConsultaListaFormularioCompradorVendedor(int IdUsuario)
        {
            string Consulta = string.Empty;

            Consulta = string.Format("SELECT FPC.Id, FPC.IdUsuario, CONCAT(usu.Nombre, ' ', usu.Apellidos) AS NombreUsuario, FPC.IdEstado, EF.Nombre_ES AS Estado, FPC.FechaFormulario, CASE WHEN EXISTS (SELECT 1 FROM [dbo].[InformacionOEA] WHERE [dbo].[InformacionOEA].IdFormulario = FPC.Id) THEN 'Sí' ELSE 'No' END AS ExisteInformacionOEA FROM [dbo].[FormularioClienteProveedores] AS FPC INNER JOIN [dbo].[tbl_Usuarios] AS usu ON (FPC.IdUsuario = usu.Id) INNER JOIN [dbo].[EstadoFormulario] AS EF ON (EF.Id = FPC.IdEstado) WHERE usu.IdCompradorVendedor={0} order by FPC.Id desc", IdUsuario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }


        public async Task<List<FormularioDto>> ListaFormulariosUsuarioOEA()
        {

            List<FormularioDto> ListaFormularios = new List<FormularioDto>();
            DataTable dtInformacion = ConsultaListaFormularioUsuarioOEA();

            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    FormularioDto objForm = new FormularioDto();
                    objForm.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objForm.IdUsuario = Convert.ToInt32(dtInformacion.Rows[rows]["IdUsuario"]);
                    objForm.NombreUsuario = dtInformacion.Rows[rows]["NombreUsuario"].ToString();
                    objForm.IdEstado = Convert.ToInt32(dtInformacion.Rows[rows]["IdEstado"]);
                    objForm.Estado = dtInformacion.Rows[rows]["Estado"].ToString();
                    objForm.Oea = dtInformacion.Rows[rows]["ExisteInformacionOEA"].ToString();
                    objForm.FechaFormulario = dtInformacion.Rows[rows]["FechaFormulario"].ToString();
                    ListaFormularios.Add(objForm);
                }
            }

            return ListaFormularios;
        }

        private DataTable ConsultaListaFormularioUsuarioOEA()
        {
            string Consulta = string.Empty;

            Consulta = string.Format("SELECT FPC.Id, FPC.IdUsuario, CONCAT(usu.Nombre, ' ', usu.Apellidos) AS NombreUsuario, FPC.IdEstado, EF.Nombre_ES AS Estado, FPC.FechaFormulario, CASE WHEN EXISTS (SELECT 1 FROM [dbo].[InformacionOEA] WHERE [dbo].[InformacionOEA].IdFormulario = FPC.Id) THEN 'Sí' ELSE 'No' END AS ExisteInformacionOEA FROM [dbo].[FormularioClienteProveedores] AS FPC INNER JOIN [dbo].[tbl_Usuarios] AS usu ON (FPC.IdUsuario = usu.Id) INNER JOIN [dbo].[EstadoFormulario] AS EF ON (EF.Id = FPC.IdEstado) ORDER BY FPC.Id DESC;");

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;

        }

        public async Task<List<FormularioDto>> ListaFormulariosContabilidad()
        {

            List<FormularioDto> ListaFormularios = new List<FormularioDto>();
            DataTable dtInformacion = ConsultaListaFormularioContabilidad();

            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    FormularioDto objForm = new FormularioDto();
                    objForm.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objForm.IdUsuario = Convert.ToInt32(dtInformacion.Rows[rows]["IdUsuario"]);
                    objForm.NombreUsuario = dtInformacion.Rows[rows]["NombreUsuario"].ToString();
                    objForm.IdEstado = Convert.ToInt32(dtInformacion.Rows[rows]["IdEstado"]);
                    objForm.Estado = dtInformacion.Rows[rows]["Estado"].ToString();
                    objForm.Oea = dtInformacion.Rows[rows]["ExisteInformacionOEA"].ToString();
                    objForm.FechaFormulario = dtInformacion.Rows[rows]["FechaFormulario"].ToString();
                    ListaFormularios.Add(objForm);
                }
            }

            return ListaFormularios;
        }

        private DataTable ConsultaListaFormularioContabilidad()
        {


            string Consulta = string.Empty;

            Consulta = string.Format("SELECT FPC.Id, FPC.IdUsuario, CONCAT(usu.Nombre, ' ', usu.Apellidos) AS NombreUsuario, FPC.IdEstado, EF.Nombre_ES AS Estado, FPC.FechaFormulario, CASE WHEN EXISTS (SELECT 1 FROM [dbo].[InformacionOEA] WHERE [dbo].[InformacionOEA].IdFormulario = FPC.Id) THEN 'Sí' ELSE 'No' END AS ExisteInformacionOEA FROM [dbo].[FormularioClienteProveedores] AS FPC INNER JOIN [dbo].[tbl_Usuarios] AS usu ON (FPC.IdUsuario = usu.Id) INNER JOIN [dbo].[EstadoFormulario] AS EF ON (EF.Id = FPC.IdEstado) where FPC.IdEstado in (3,4,5,7,8,9) order by FPC.Id desc");

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;



        }


        public async Task<List<FormularioDto>> ListaFormulariosControlInterno()
        {

            List<FormularioDto> ListaFormularios = new List<FormularioDto>();
            DataTable dtInformacion = ConsultaListaFormularioControlInterno();

            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    FormularioDto objForm = new FormularioDto();
                    objForm.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objForm.IdUsuario = Convert.ToInt32(dtInformacion.Rows[rows]["IdUsuario"]);
                    objForm.NombreUsuario = dtInformacion.Rows[rows]["NombreUsuario"].ToString();
                    objForm.IdEstado = Convert.ToInt32(dtInformacion.Rows[rows]["IdEstado"]);
                    objForm.Estado = dtInformacion.Rows[rows]["Estado"].ToString();
                    objForm.Oea = dtInformacion.Rows[rows]["ExisteInformacionOEA"].ToString();
                    objForm.FechaFormulario = dtInformacion.Rows[rows]["FechaFormulario"].ToString();
                    ListaFormularios.Add(objForm);
                }
            }

            return ListaFormularios;
        }

        private DataTable ConsultaListaFormularioControlInterno()
        {
            string Consulta = string.Empty;
            Consulta = string.Format("SELECT FPC.Id, FPC.IdUsuario, CONCAT(usu.Nombre, ' ', usu.Apellidos) AS NombreUsuario, FPC.IdEstado, EF.Nombre_ES AS Estado, FPC.FechaFormulario, CASE WHEN EXISTS (SELECT 1 FROM [dbo].[InformacionOEA] WHERE [dbo].[InformacionOEA].IdFormulario = FPC.Id) THEN 'Sí' ELSE 'No' END AS ExisteInformacionOEA FROM [dbo].[FormularioClienteProveedores] AS FPC INNER JOIN [dbo].[tbl_Usuarios] AS usu ON (FPC.IdUsuario = usu.Id) INNER JOIN [dbo].[EstadoFormulario] AS EF ON (EF.Id = FPC.IdEstado) where FPC.IdEstado in (5,7) order by FPC.Id desc");
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }

        public async Task<List<FormularioDto>> ListaFormulariosOficialCumplimiento()
        {

            List<FormularioDto> ListaFormularios = new List<FormularioDto>();
            DataTable dtInformacion = ConsultaListaFormularioOficialCumplimiento();
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    FormularioDto objForm = new FormularioDto();
                    objForm.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objForm.IdUsuario = Convert.ToInt32(dtInformacion.Rows[rows]["IdUsuario"]);
                    objForm.NombreUsuario = dtInformacion.Rows[rows]["NombreUsuario"].ToString();
                    objForm.IdEstado = Convert.ToInt32(dtInformacion.Rows[rows]["IdEstado"]);
                    objForm.Estado = dtInformacion.Rows[rows]["Estado"].ToString();
                    objForm.Oea = dtInformacion.Rows[rows]["ExisteInformacionOEA"].ToString();
                    objForm.FechaFormulario = dtInformacion.Rows[rows]["FechaFormulario"].ToString();
                    ListaFormularios.Add(objForm);
                }
            }

            return ListaFormularios;
        }

        private DataTable ConsultaListaFormularioOficialCumplimiento()
        {


            string Consulta = string.Empty;

            Consulta = string.Format("SELECT FPC.Id, FPC.IdUsuario, CONCAT(usu.Nombre, ' ', usu.Apellidos) AS NombreUsuario, FPC.IdEstado, EF.Nombre_ES AS Estado, FPC.FechaFormulario, CASE WHEN EXISTS (SELECT 1 FROM [dbo].[InformacionOEA] WHERE [dbo].[InformacionOEA].IdFormulario = FPC.Id) THEN 'Sí' ELSE 'No' END AS ExisteInformacionOEA FROM [dbo].[FormularioClienteProveedores] AS FPC INNER JOIN [dbo].[tbl_Usuarios] AS usu ON (FPC.IdUsuario = usu.Id) INNER JOIN [dbo].[EstadoFormulario] AS EF ON (EF.Id = FPC.IdEstado) where FPC.IdEstado in (8) order by FPC.Id desc");

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;



        }



        public async Task<List<FormularioDto>> ListaFormulariosbyClienteProveedor(int IdUsuario, string Lang)
        {

            List<FormularioDto> ListaFormularios = new List<FormularioDto>();
            DataTable dtInformacion = ConsultaListaFormulariobyClienteProveedor(IdUsuario);

            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    FormularioDto objForm = new FormularioDto();
                    objForm.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objForm.IdUsuario = Convert.ToInt32(dtInformacion.Rows[rows]["IdUsuario"]);
                    objForm.NombreUsuario = dtInformacion.Rows[rows]["NombreUsuario"].ToString();
                    objForm.IdEstado = Convert.ToInt32(dtInformacion.Rows[rows]["IdEstado"]);
                    objForm.Estado = dtInformacion.Rows[rows]["Estado_" + Lang].ToString();
                    objForm.FechaFormulario = dtInformacion.Rows[rows]["FechaFormulario"].ToString();
                    ListaFormularios.Add(objForm);
                }
            }

            return ListaFormularios;
        }
        private DataTable ConsultaListaFormulariobyClienteProveedor(int IdUsuario)
        {


            string Consulta = string.Empty;

            Consulta = string.Format("select FPC.Id, FPC.IdUsuario, CONCAT(usu.Nombre,' ' , usu.Apellidos) as NombreUsuario, FPC.IdEstado,EF.Nombre_ES as Estado_ES, EF.Nombre_EN as Estado_EN,FPC.FechaFormulario from [dbo].[FormularioClienteProveedores] as FPC inner join [dbo].[tbl_Usuarios] as usu ON (FPC.IdUsuario=usu.Id) inner join [dbo].[EstadoFormulario] as EF ON (EF.Id=FPC.IdEstado) where FPC.IdUsuario={0} order by FPC.Id desc", IdUsuario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;



        }



        public async Task<DatosGeneralesDto> ConsultaDatosGenerales(int IdFormulario)

        {

            string Consulta = string.Empty;
            DatosGeneralesDto objeto = new DatosGeneralesDto();
            Consulta = string.Format("SELECT [Id],[IdFormulario],[Empresa],[TipoSolicitud] ,[ClaseTercero] ,[CategoriaTercero] ,[NombreRazonSocial] ,[TipoIdentificacion] ,[NumeroIdentificacion],[DigitoVerificacion],[Pais] ,[Ciudad] ,[TamanoTercero] ,[RazonSocial],[DireccionPrincipal],[CodigoPostal] ,[CorreoElectronico],[Telefono],[ObligadoFacturarElectronicamente],[CorreoElectronicoFacturaEletronica],[SucursalOtroPais] ,[OtroPais],[JsonPreguntasPep], [EstadoCivil], [ConyugeIdentificacion], [tipoPago], [CertBASC], [CertOEA], [CertCTPAT], [CertOtros], [CertNinguno] FROM [dbo].[DatosGenerales] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                objeto.IdFormulario = IdFormulario;
                objeto.FechaDiligenciamiento = "";
                objeto.Empresa = Convert.ToInt32(dtInformacion.Rows[0]["Empresa"]);
                objeto.TipoSolicitud = Convert.ToInt32(dtInformacion.Rows[0]["TipoSolicitud"]);
                objeto.ClaseTercero = Convert.ToInt32(dtInformacion.Rows[0]["ClaseTercero"]);
                objeto.CategoriaTercero = Convert.ToInt32(dtInformacion.Rows[0]["CategoriaTercero"]);
                objeto.NombreRazonSocial = dtInformacion.Rows[0]["NombreRazonSocial"].ToString();
                objeto.TipoIdentificacion = Convert.ToInt32(dtInformacion.Rows[0]["TipoIdentificacion"]);
                objeto.NumeroIdentificacion = dtInformacion.Rows[0]["NumeroIdentificacion"].ToString();
                objeto.DigitoVerificacion = dtInformacion.Rows[0]["DigitoVerificacion"].ToString();

                objeto.Pais = Convert.ToInt32(dtInformacion.Rows[0]["Pais"]);
                objeto.Ciudad = dtInformacion.Rows[0]["Ciudad"].ToString();
                objeto.TamanoTercero = Convert.ToInt32(dtInformacion.Rows[0]["TamanoTercero"]);
                objeto.ActividadEconimoca = Convert.ToInt32(dtInformacion.Rows[0]["RazonSocial"]);
                objeto.DireccionPrincipal = dtInformacion.Rows[0]["DireccionPrincipal"].ToString();
                objeto.CodigoPostal = dtInformacion.Rows[0]["CodigoPostal"].ToString();

                objeto.CorreoElectronico = dtInformacion.Rows[0]["CorreoElectronico"].ToString();
                objeto.Telefono = dtInformacion.Rows[0]["Telefono"].ToString();
                objeto.ObligadoFE = Convert.ToInt32(dtInformacion.Rows[0]["ObligadoFacturarElectronicamente"]);
                objeto.CorreoElectronicoFE = dtInformacion.Rows[0]["CorreoElectronicoFacturaEletronica"].ToString();
                objeto.TieneSucursalesOtrosPaises = Convert.ToInt32(dtInformacion.Rows[0]["SucursalOtroPais"]);
                objeto.PaisesOtrasSucursales = dtInformacion.Rows[0]["OtroPais"].ToString();
                objeto.PreguntasAdicionales = dtInformacion.Rows[0]["JsonPreguntasPep"];
                objeto.EstadoCivil = dtInformacion.Rows[0]["EstadoCivil"].ToString();
                objeto.ConyugeIdentificacion = dtInformacion.Rows[0]["ConyugeIdentificacion"].ToString();
                var valorDb = dtInformacion.Rows[0]["tipoPago"];
                objeto.tipoPago = valorDb == DBNull.Value ? false : Convert.ToBoolean(valorDb);
                objeto.CertBASC = Convert.ToBoolean(dtInformacion.Rows[0]["CertBASC"]);
                objeto.CertOEA = Convert.ToBoolean(dtInformacion.Rows[0]["CertOEA"]);
                objeto.CertCTPAT = Convert.ToBoolean(dtInformacion.Rows[0]["CertCTPAT"]);
                objeto.CertOtros = Convert.ToBoolean(dtInformacion.Rows[0]["CertOtros"]);
                objeto.CertNinguno = Convert.ToBoolean(dtInformacion.Rows[0]["CertNinguno"]);


                return objeto;
            }

            return null;

        }


        private DataTable ConsultaListaFormulario()
        {
            string Consulta = string.Empty;

            Consulta = string.Format("SELECT FPC.Id, FPC.IdUsuario, CONCAT(usu.Nombre, ' ', usu.Apellidos) AS NombreUsuario, FPC.IdEstado, EF.Nombre_ES AS Estado, FPC.FechaFormulario, CASE WHEN EXISTS (SELECT 1 FROM [dbo].[InformacionOEA] WHERE [dbo].[InformacionOEA].IdFormulario = FPC.Id) THEN 'Sí' ELSE 'No' END AS ExisteInformacionOEA FROM [dbo].[FormularioClienteProveedores] AS FPC INNER JOIN [dbo].[tbl_Usuarios] AS usu ON (FPC.IdUsuario = usu.Id) INNER JOIN [dbo].[EstadoFormulario] AS EF ON (EF.Id = FPC.IdEstado) ORDER BY FPC.Id DESC;");

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;



        }

        public async Task<bool> GuardarDatosGenerales(DatosGeneralesDto objRegistro)
        {
            bool Respeusta = false;

            if (ExisteDatosGenerales(objRegistro.Id, objRegistro.IdFormulario) || (objRegistro.Id != 0))
            {
                Respeusta = EditaDatosGenerales(objRegistro);

            }
            else
            {
                Respeusta = GuardaDatosGeneralesFist(objRegistro);
            }

            return Respeusta;
        }



        private bool GuardaDatosGeneralesFist(DatosGeneralesDto objRegistro)
        {
            bool respuesta = false;
            try
            {


                string query = "insert into [dbo].[DatosGenerales] " +
                          "VALUES (@IdFormulario, @Empresa, @TipoSolicitud, @ClaseTercero, @CategoriaTercero, @NombreRazonSocial, @TipoIdentificacion, @NumeroIdentificacion, @DigitoVerificacion, @Pais, @Ciudad, @TamanoTercero, @RazonSocial, @DireccionPrincipal, @CodigoPostal, @CorreoElectronico, @Telefono, @ObligadoFacturarElectronicamente, @CorreoElectronicoFacturaEletronica, @SucursalOtroPais,@OtroPais,@JsonPreguntasPep,@EstadoCivil,@ConyugeIdentificacion,@tipoPago,@CertBASC,@CertOEA,@CertCTPAT,@CertOtros,@CertNinguno)";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@Empresa ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Empresa },
                    new SqlParameter() { ParameterName = "@TipoSolicitud ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TipoSolicitud },
                     new SqlParameter() { ParameterName = "@ClaseTercero ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ClaseTercero },
                      new SqlParameter() { ParameterName = "@CategoriaTercero ", SqlDbType = SqlDbType.Int, Value =  objRegistro.CategoriaTercero },
                    new SqlParameter() { ParameterName = "@EstadoCivil ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.EstadoCivil },
                    new SqlParameter() { ParameterName = "@ConyugeIdentificacion ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ConyugeIdentificacion },
                    new SqlParameter() { ParameterName = "@tipoPago ", SqlDbType = SqlDbType.Bit, Value =  (object)objRegistro.tipoPago ?? DBNull.Value },
                    new SqlParameter() { ParameterName = "@CertBASC ", SqlDbType = SqlDbType.Bit, Value = objRegistro.CertBASC },
                    new SqlParameter() { ParameterName = "@CertOEA ", SqlDbType =  SqlDbType.Bit, Value = objRegistro.CertOEA },
                    new SqlParameter() { ParameterName = "@CertCTPAT ", SqlDbType =  SqlDbType.Bit, Value = objRegistro.CertCTPAT },
                    new SqlParameter() { ParameterName = "@CertOtros ", SqlDbType =  SqlDbType.Bit, Value = objRegistro.CertOtros },
                    new SqlParameter() { ParameterName = "@CertNinguno ", SqlDbType =  SqlDbType.Bit, Value = objRegistro.CertNinguno },

                       new SqlParameter() { ParameterName = "@NombreRazonSocial ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreRazonSocial },
                       new SqlParameter() { ParameterName = "@TipoIdentificacion ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TipoIdentificacion },
                       new SqlParameter() { ParameterName = "@NumeroIdentificacion ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NumeroIdentificacion },
                       new SqlParameter() { ParameterName = "@DigitoVerificacion ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.DigitoVerificacion },
                       new SqlParameter() { ParameterName = "@Pais ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Pais },
                       new SqlParameter() { ParameterName = "@Ciudad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Ciudad },
                       new SqlParameter() { ParameterName = "@TamanoTercero ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TamanoTercero },
                       new SqlParameter() { ParameterName = "@RazonSocial ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ActividadEconimoca },
                              new SqlParameter() { ParameterName = "@DireccionPrincipal ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.DireccionPrincipal },
                       new SqlParameter() { ParameterName = "@CodigoPostal ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CodigoPostal },
                       new SqlParameter() { ParameterName = "@CorreoElectronico", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronico },
                       new SqlParameter() { ParameterName = "@Telefono", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Telefono },

                        new SqlParameter() { ParameterName = "@ObligadoFacturarElectronicamente", SqlDbType = SqlDbType.Int, Value =  objRegistro.ObligadoFE },
                     new SqlParameter() { ParameterName = "@CorreoElectronicoFacturaEletronica", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronicoFE },
                    new SqlParameter() { ParameterName = "@SucursalOtroPais", SqlDbType = SqlDbType.Int, Value =  objRegistro.TieneSucursalesOtrosPaises },
                        new SqlParameter() { ParameterName = "@OtroPais", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.PaisesOtrasSucursales },
                        new SqlParameter() { ParameterName = "@JsonPreguntasPep", SqlDbType = SqlDbType.NVarChar, Value = objRegistro.PreguntasAdicionales.ToString()},



                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                respuesta = true;
                cDataBase.desconectar();



            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error GuardaDatosGeneralesFist");
                respuesta = false;


            }

            return respuesta;

        }


        private bool EditaDatosGenerales(DatosGeneralesDto objRegistro)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[DatosGenerales] " +
               "SET [Empresa] = @Empresa, " +
               "[TipoSolicitud] = @TipoSolicitud, " +
               "[ClaseTercero] = @ClaseTercero, " +
               "[CategoriaTercero] = @CategoriaTercero, " +
               "[NombreRazonSocial] = @NombreRazonSocial, " +
               "[TipoIdentificacion] = @TipoIdentificacion, " +
               "[NumeroIdentificacion] = @NumeroIdentificacion, " +
               "[DigitoVerificacion]= @DigitoVerificacion, " +
               "[Pais] = @Pais, " +
               "[Ciudad] = @Ciudad, " +
               "[TamanoTercero] = @TamanoTercero, " +
               "[RazonSocial] = @RazonSocial, " +
               "[DireccionPrincipal] = @DireccionPrincipal, " +
               "[CodigoPostal] = @CodigoPostal, " +
               "[CorreoElectronico] = @CorreoElectronico, " +

               "[Telefono] = @Telefono, " +
               "[ObligadoFacturarElectronicamente] = @ObligadoFacturarElectronicamente, " +
               "[CorreoElectronicoFacturaEletronica] = @CorreoFE, " +
               "[SucursalOtroPais] =@SucursalOtroPais, " +
               "[OtroPais] = @OtroPais, " +
                "[JsonPreguntasPep] = @JsonPreguntasPep, " +
                "[EstadoCivil] = @EstadoCivil, " +
                "[ConyugeIdentificacion] = @ConyugeIdentificacion, " +
                "[tipoPago] = @tipoPago, " +
                "[CertBASC] = @CertBASC, " +
                "[CertOEA] = @CertOEA, " +
                "[CertCTPAT] = @CertCTPAT, " +
                "[CertOtros] = @CertOtros, " +
                "[CertNinguno] = @CertNinguno " +
               "WHERE [Id] = @Id and [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                    new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value =  objRegistro.Id },
                    new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@Empresa ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Empresa },
                    new SqlParameter() { ParameterName = "@TipoSolicitud ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TipoSolicitud },
                    new SqlParameter() { ParameterName = "@ClaseTercero ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ClaseTercero },
                    new SqlParameter() { ParameterName = "@CategoriaTercero ", SqlDbType = SqlDbType.Int, Value =  objRegistro.CategoriaTercero },
                    new SqlParameter() { ParameterName = "@EstadoCivil ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.EstadoCivil },
                    new SqlParameter() { ParameterName = "@ConyugeIdentificacion ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ConyugeIdentificacion },
                    new SqlParameter() { ParameterName = "@tipoPago ", SqlDbType = SqlDbType.Bit, Value =  (object)objRegistro.tipoPago ?? DBNull.Value },
                    new SqlParameter() { ParameterName = "@CertBASC", SqlDbType = SqlDbType.Bit, Value = objRegistro.CertBASC },
                    new SqlParameter() { ParameterName = "@CertOEA", SqlDbType =  SqlDbType.Bit, Value = objRegistro.CertOEA },
                    new SqlParameter() { ParameterName = "@CertCTPAT", SqlDbType =  SqlDbType.Bit, Value = objRegistro.CertCTPAT },
                    new SqlParameter() { ParameterName = "@CertOtros", SqlDbType =  SqlDbType.Bit, Value = objRegistro.CertOtros },
                    new SqlParameter() { ParameterName = "@CertNinguno", SqlDbType =  SqlDbType.Bit, Value = objRegistro.CertNinguno },


                       new SqlParameter() { ParameterName = "@NombreRazonSocial ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreRazonSocial },
                       new SqlParameter() { ParameterName = "@TipoIdentificacion ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TipoIdentificacion },
                       new SqlParameter() { ParameterName = "@NumeroIdentificacion ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NumeroIdentificacion },
                       new SqlParameter() { ParameterName = "@DigitoVerificacion", SqlDbType = SqlDbType.VarChar, Value =  (object)objRegistro.DigitoVerificacion ?? (object)DBNull.Value },
                       new SqlParameter() { ParameterName = "@Pais ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Pais },
                       new SqlParameter() { ParameterName = "@Ciudad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Ciudad },
                       new SqlParameter() { ParameterName = "@TamanoTercero ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TamanoTercero },
                       new SqlParameter() { ParameterName = "@RazonSocial ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ActividadEconimoca },
                              new SqlParameter() { ParameterName = "@DireccionPrincipal ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.DireccionPrincipal },
                       new SqlParameter() { ParameterName = "@CodigoPostal ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CodigoPostal },
                       new SqlParameter() { ParameterName = "@CorreoElectronico", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronico },
                       new SqlParameter() { ParameterName = "@Telefono", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Telefono },

                        new SqlParameter() { ParameterName = "@ObligadoFacturarElectronicamente", SqlDbType = SqlDbType.Int, Value =  objRegistro.ObligadoFE },
                         new SqlParameter() { ParameterName = "@CorreoFE", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronicoFE },
                        new SqlParameter() { ParameterName = "@SucursalOtroPais", SqlDbType = SqlDbType.Int, Value =  objRegistro.TieneSucursalesOtrosPaises },
                        new SqlParameter() { ParameterName = "@OtroPais", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.PaisesOtrasSucursales },
                         new SqlParameter() { ParameterName = "@JsonPreguntasPep", SqlDbType = SqlDbType.NVarChar, Value = objRegistro.PreguntasAdicionales.ToString()},


                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el DatosGenerales");
            }
            return respuesta;


        }



        private bool ExisteDatosGenerales(int Id, int IdFormulario)
        {


            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[DatosGenerales] where Id={0} and IdFormulario={1}", Id, IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public async Task<bool> GuardaInformacionContactos(List<DatosContactoDto> objRegistro)
        {
            bool resultado = false;

            try
            {


                foreach (DatosContactoDto contacto in objRegistro)
                {
                    if (existecontacto(contacto) || contacto.Id != 0)
                    {
                        EditaContacto(contacto);
                    }
                    else
                    {
                        GuardaContacto(contacto);
                    }
                }
                resultado = true;
            }
            catch (Exception ex)
            {
                resultado = false;
                throw new InvalidOperationException("error al CreaR DatosContacto");
            }
            return resultado;
        }


        private bool existecontacto(DatosContactoDto objRegistro)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[DatosContacto] where Id={0} and IdFormulario={1}", objRegistro.Id, objRegistro.IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GuardaContacto(DatosContactoDto objRegistro)
        {
            try
            {
                string querydelete = "Delete from [dbo].[DatosContacto] where IdFormulario=@IdFormulario";


                string query = "insert into [dbo].[DatosContacto] " +
                          "VALUES (@IdFormulario, @NombreContacto, @Cargo, @Area, @Telefono, @CorreoElectronico, @Ciudad, @Direccion)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@NombreContacto ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreContacto },
                    new SqlParameter() { ParameterName = "@Cargo ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CargoContacto },
                    new SqlParameter() { ParameterName = "@Area ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.AreaContacto },
                    new SqlParameter() { ParameterName = "@Telefono ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.TelefonoContacto },
                    new SqlParameter() { ParameterName = "@CorreoElectronico ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronico },
                    new SqlParameter() { ParameterName = "@Ciudad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Ciudad },
                    new SqlParameter() { ParameterName = "@Direccion ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Direccion },

                };
                cDataBase.conectar();


                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
            }

        }


        private void EliminarDatosContacto(int IdFormulario)
        {
            try
            {
                string querydelete = "Delete from [dbo].[DatosContacto] where IdFormulario=@IdFormulario";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },

                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(querydelete, parametros);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
            }

        }
        private void EditaContacto(DatosContactoDto objRegistro)
        {
            string strConsulta;
            try
            {
                strConsulta = "UPDATE [dbo].[DatosContacto] " +
               "SET [NombreContacto] = @NombreContacto, " +
               "[Cargo] = @Cargo, " +
               "[Area] = @Area, " +
               "[Telefono] = @Telefono, " +
               "[CorreoElectronico] = @CorreoElectronico, " +
               "[Ciudad] = @Ciudad, " +
               "[Direccion] = @Direccion " +

               "WHERE [Id] = @Id and [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                    new SqlParameter() { ParameterName = "@Id ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Id },
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@NombreContacto ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreContacto },
                    new SqlParameter() { ParameterName = "@Cargo ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CargoContacto },
                    new SqlParameter() { ParameterName = "@Area ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.AreaContacto },
                    new SqlParameter() { ParameterName = "@Telefono ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.TelefonoContacto },
                    new SqlParameter() { ParameterName = "@CorreoElectronico ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronico },
                    new SqlParameter() { ParameterName = "@Ciudad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Ciudad },
                    new SqlParameter() { ParameterName = "@Direccion ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Direccion },

                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);

                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el DatosGenerales");
            }
        }


        public async Task<List<DatosContactoDto>> ListaDatosContacto(int IdFormulario)
        {

            string Consulta = string.Empty;
            List<DatosContactoDto> listobjeto = new List<DatosContactoDto>();
            Consulta = string.Format("SELECT [Id],[IdFormulario],[NombreContacto],[Cargo],[Area],[Telefono],[CorreoElectronico], [Ciudad], [Direccion] FROM [dbo].[DatosContacto] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    DatosContactoDto objDato = new DatosContactoDto();

                    objDato.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objDato.IdFormulario = IdFormulario;
                    objDato.NombreContacto = dtInformacion.Rows[rows]["NombreContacto"].ToString().Trim();
                    objDato.CargoContacto = dtInformacion.Rows[rows]["Cargo"].ToString().Trim();
                    objDato.AreaContacto = dtInformacion.Rows[rows]["Area"].ToString().Trim();
                    objDato.TelefonoContacto = dtInformacion.Rows[rows]["Telefono"].ToString().Trim();
                    objDato.CorreoElectronico = dtInformacion.Rows[rows]["CorreoElectronico"].ToString().Trim();
                    objDato.Ciudad = dtInformacion.Rows[rows]["Ciudad"].ToString().Trim();
                    objDato.Direccion = dtInformacion.Rows[rows]["Direccion"].ToString().Trim();


                    listobjeto.Add(objDato);
                }


                return listobjeto;
            }

            return null;


        }



        public async Task<bool> GuardaReferenciaComercialBanc(List<ReferenciaComercialesBancariasDtol> objRegistro)
        {
            bool resultado = false;

            try
            {


                foreach (ReferenciaComercialesBancariasDtol referencia in objRegistro)
                {

                    if (existeReferencia(referencia) || referencia.Id != 0)
                    {
                        EditaReferenciaComercial(referencia);
                    }
                    else
                    {
                        GuardaReferencia(referencia);
                    }

                }

                resultado = true;
            }
            catch (Exception ex)
            {
                resultado = false;
                throw new InvalidOperationException("error al CreaR DatosContacto");
            }
            return resultado;
        }

        private void EditaReferenciaComercial(ReferenciaComercialesBancariasDtol objRegistro)
        {
            string strConsulta;
            try
            {
                strConsulta = "UPDATE [dbo].[ReferenciasComercialesBancarias] " +
               "SET [TipoReferencia] = @TipoReferencia, " +
               "[NombreCompleto] = @NombreCompleto, " +
               "[Ciudad] = @Ciudad, " +
               "[Telefono] = @Telefono, " +
               "[ValorAnualCompras] = @ValorAnualCompras, " +
               "[Cupo] = @Cupo, " +
               "[Plazo] = @Plazo " +

               "WHERE [Id] = @Id and [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                    new SqlParameter() { ParameterName = "@Id ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Id },
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@TipoReferencia ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TipoReferencia },
                    new SqlParameter() { ParameterName = "@NombreCompleto ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreCompleto },
                    new SqlParameter() { ParameterName = "@Ciudad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Ciudad },
                    new SqlParameter() { ParameterName = "@Telefono ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Telefono },
                    new SqlParameter() { ParameterName = "@ValorAnualCompras ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ValorAnualCompras },
                    new SqlParameter() { ParameterName = "@Cupo ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Cupo },
                    new SqlParameter() { ParameterName = "@Plazo ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Plazo },


                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);

                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el Referencia");
            }
        }

        private void GuardaReferencia(ReferenciaComercialesBancariasDtol objRegistro)
        {
            try
            {
                string query = "insert into [dbo].[ReferenciasComercialesBancarias] " +
                         "VALUES (@IdFormulario, @TipoReferencia, @NombreCompleto, @Ciudad, @Telefono, @ValorAnualCompras, @Cupo, @Plazo)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@TipoReferencia ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TipoReferencia },
                    new SqlParameter() { ParameterName = "@NombreCompleto ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreCompleto },
                    new SqlParameter() { ParameterName = "@Ciudad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Ciudad },
                    new SqlParameter() { ParameterName = "@Telefono ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Telefono },
                    new SqlParameter() { ParameterName = "@ValorAnualCompras ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ValorAnualCompras },
                    new SqlParameter() { ParameterName = "@Cupo ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Cupo },
                    new SqlParameter() { ParameterName = "@Plazo ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Plazo },


                };
                cDataBase.conectar();


                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
            }

        }

        private void EliminarReferenca(int IdFormulario)
        {
            try
            {
                string querydelete = "Delete from [dbo].[ReferenciasComercialesBancarias] where IdFormulario=@IdFormulario";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },

                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(querydelete, parametros);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
            }

        }

        private bool existeReferencia(ReferenciaComercialesBancariasDtol objRegistro)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[ReferenciasComercialesBancarias] where Id={0} and IdFormulario={1}", objRegistro.Id, objRegistro.IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public async Task<List<ReferenciaComercialesBancariasDtol>> ListaReferenciasComercialesBan(int IdFormulario)
        {

            string Consulta = string.Empty;
            List<ReferenciaComercialesBancariasDtol> listobjeto = new List<ReferenciaComercialesBancariasDtol>();
            Consulta = string.Format("SELECT  [Id] ,[IdFormulario]  ,[TipoReferencia],[NombreCompleto] ,[Ciudad] ,[Telefono], [ValorAnualCompras], [Cupo], [Plazo]  FROM [dbo].[ReferenciasComercialesBancarias] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    ReferenciaComercialesBancariasDtol objDato = new ReferenciaComercialesBancariasDtol();

                    objDato.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objDato.IdFormulario = IdFormulario;
                    objDato.TipoReferencia = Convert.ToInt32(dtInformacion.Rows[rows]["TipoReferencia"]);
                    objDato.NombreCompleto = dtInformacion.Rows[rows]["NombreCompleto"].ToString().Trim();
                    objDato.Ciudad = dtInformacion.Rows[rows]["Ciudad"].ToString().Trim();
                    objDato.Telefono = dtInformacion.Rows[rows]["Telefono"].ToString().Trim();
                    objDato.ValorAnualCompras = dtInformacion.Rows[rows]["ValorAnualCompras"].ToString().Trim();
                    objDato.Cupo = dtInformacion.Rows[rows]["Cupo"].ToString().Trim();
                    objDato.Plazo = dtInformacion.Rows[rows]["Plazo"].ToString().Trim();

                    listobjeto.Add(objDato);
                }


                return listobjeto;
            }

            return null;


        }




        public async Task<bool> GuardaDatosPago(DatosPagosDto objRegistro)
        {
            bool Respuesta = false;

            try
            {

                if (existeDatosPagosDto(objRegistro.Id, objRegistro.IdFormulario) || (objRegistro.Id != 0))
                {
                    Respuesta = editaDatosPago(objRegistro);
                }
                else
                {
                    Respuesta = GuardaDatosPgo(objRegistro);
                }


            }
            catch (Exception ex)
            {
                Respuesta = false;
                throw new InvalidOperationException("error al CreaR DatosContacto");
            }
            return Respuesta;
        }

        private bool existeDatosPagosDto(int Id, int IdFormulario)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[DatosDePagos] where Id={0} and IdFormulario={1}", Id, IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        private void EliminarDatosdePago(int IdFormulario)
        {
            try
            {
                string querydelete = "Delete from [dbo].[DatosDePagos] where IdFormulario=@IdFormulario";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(querydelete, parametros);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
            }

        }

        private bool GuardaDatosPgo(DatosPagosDto objRegistro)
        {

            try
            {
                string query = "insert into [dbo].[DatosDePagos] " +
                         "VALUES (@IdFormulario, @NombreBanco, @NumeroCuenta, @TipoCuenta, @CodigoSwift , @Ciudad , @Pais, @CorreoElectronico, @Sucursal)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@NombreBanco ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreBanco },
                    new SqlParameter() { ParameterName = "@NumeroCuenta ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NumeroCuenta },
                    new SqlParameter() { ParameterName = "@TipoCuenta ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TipoCuenta },
                    new SqlParameter() { ParameterName = "@CodigoSwift ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CodigoSwift },
                    new SqlParameter() { ParameterName = "@Ciudad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Ciudad },
                    new SqlParameter() { ParameterName = "@Pais ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Pais },
                    new SqlParameter() { ParameterName = "@CorreoElectronico ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronico },
                    new SqlParameter() { ParameterName = "@Sucursal ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Sucursal },


                };
                cDataBase.conectar();


                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();
                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
                return false;
            }

        }


        private bool editaDatosPago(DatosPagosDto objRegistro)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[DatosDePagos] " +
               "SET [NombreBanco] = @NombreBanco, " +
               "[NumeroCuenta] = @NumeroCuenta, " +
               "[TipoCuenta] = @TipoCuenta, " +
               "[CodigoSwift] = @CodigoSwift, " +
               "[Ciudad] = @Ciudad, " +
               "[Pais] = @Pais, " +
               "[CorreoElectronico] = @CorreoElectronico, " +
               "[Sucursal] = @Sucursal " +
               "WHERE [Id] = @Id and [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                     new SqlParameter() { ParameterName = "@Id ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Id },
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@NombreBanco ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreBanco },
                    new SqlParameter() { ParameterName = "@NumeroCuenta ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NumeroCuenta },
                    new SqlParameter() { ParameterName = "@TipoCuenta ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TipoCuenta },
                    new SqlParameter() { ParameterName = "@CodigoSwift ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CodigoSwift },
                    new SqlParameter() { ParameterName = "@Ciudad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Ciudad },
                    new SqlParameter() { ParameterName = "@Pais ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Pais },
                    new SqlParameter() { ParameterName = "@CorreoElectronico ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronico },
                    new SqlParameter() { ParameterName = "@Sucursal ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Sucursal }
                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el DatosGenerales");
            }
            return respuesta;


        }



        public async Task<DatosPagosDto> ConsultaDatosPago(int IdFormulario)
        {

            string Consulta = string.Empty;
            DatosPagosDto objeto = new DatosPagosDto();
            Consulta = string.Format("SELECT [Id],[IdFormulario] ,[NombreBanco]  ,[NumeroCuenta]  ,[TipoCuenta] ,[CodigoSwift]  ,[Ciudad] ,[Pais] ,[CorreoElectronico], [Sucursal] FROM [dbo].[DatosDePagos] where IdFormulario={0}", IdFormulario);
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                objeto.IdFormulario = IdFormulario;
                objeto.NombreBanco = dtInformacion.Rows[0]["NombreBanco"].ToString();
                objeto.NumeroCuenta = dtInformacion.Rows[0]["NumeroCuenta"].ToString();
                objeto.TipoCuenta = Convert.ToInt32(dtInformacion.Rows[0]["TipoCuenta"]);
                objeto.CodigoSwift = dtInformacion.Rows[0]["CodigoSwift"].ToString();
                objeto.Ciudad = dtInformacion.Rows[0]["Ciudad"].ToString();
                objeto.Pais = Convert.ToInt32(dtInformacion.Rows[0]["Pais"]);
                objeto.CorreoElectronico = dtInformacion.Rows[0]["CorreoElectronico"].ToString();
                objeto.Sucursal = dtInformacion.Rows[0]["Sucursal"].ToString();

                return objeto;
            }

            return null;

        }


        public async Task<bool> GuardaCumplimientoNormativo(CumplimientoNormativoDto objRegistro)
        {
            bool Respuesta = false;
            try
            {
                if (existeCumplientoNormativo(objRegistro.Id, objRegistro.IdFormulario))
                {
                    Respuesta = EditaCumpliomientoNormativo(objRegistro);
                }
                else
                {
                    Respuesta = GuardaCumplimientoNor(objRegistro);
                }
            }
            catch (Exception ex)
            {
                Respuesta = false;
                throw new InvalidOperationException("error al CreaR DatosContacto");
            }
            return Respuesta;
        }
        public async Task<CumplimientoNormativoDto> ConsultaCumplimientoNormativo(int IdFormulario)
        {
            string Consulta = string.Format(@"SELECT [id], [id_formulario],[sometida_sagrilaft], [sometida_otro_sistema],[adhesion_politicas_samsung], [no_invest_sancion_laftfpadm],
                                                [no_transacciones_ilicitas], [acepta_monitoreo_info], [no_listas_restrictivas], [correo_reportar_incidentes] FROM [dbo].[CumplimientoNormativo]
                                            WHERE id_formulario = {0}", IdFormulario);

            DataTable dtInformacion = new DataTable();
            CumplimientoNormativoDto objeto = null;

            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                DataRow row = dtInformacion.Rows[0];
                objeto = new CumplimientoNormativoDto();

                objeto.Id = Convert.ToInt32(row["id"]);
                objeto.IdFormulario = Convert.ToInt32(row["id_formulario"]);
                objeto.sometida_sagrilaft = Convert.ToBoolean(row["sometida_sagrilaft"]);
                objeto.sometida_otro_sistema = Convert.ToBoolean(row["sometida_otro_sistema"]);
                objeto.adhesion_politicas_samsung = Convert.ToBoolean(row["adhesion_politicas_samsung"]);
                objeto.no_invest_sancion_laftfpadm = Convert.ToBoolean(row["no_invest_sancion_laftfpadm"]);
                objeto.no_transacciones_ilicitas = Convert.ToBoolean(row["no_transacciones_ilicitas"]);
                objeto.acepta_monitoreo_info = Convert.ToBoolean(row["acepta_monitoreo_info"]);
                objeto.no_listas_restrictivas = Convert.ToBoolean(row["no_listas_restrictivas"]);

                if (row["correo_reportar_incidentes"] != DBNull.Value)
                    objeto.correo_reportar_incidentes = row["correo_reportar_incidentes"].ToString();
                else
                    objeto.correo_reportar_incidentes = null;
            }

            return objeto;
        }

        private bool existeCumplientoNormativo(int Id, int IdFormulario)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[CumplimientoNormativo] where Id={0} and id_formulario={1}", Id, IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        private void EliminarCumplimientoNormativo(int IdFormulario)
        {
            try
            {
                string querydelete = "Delete from [dbo].[CumplimientoNormativo] where id_formulario=@IdFormulario";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(querydelete, parametros);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
            }

        }
        private bool GuardaCumplimientoNor(CumplimientoNormativoDto objRegistro)
        {
            try
            {
                string query = @"INSERT INTO [dbo].[CumplimientoNormativo]
                                (
                                    [id_formulario],
                                    [sometida_sagrilaft],
                                    [sometida_otro_sistema],
                                    [adhesion_politicas_samsung],
                                    [no_invest_sancion_laftfpadm],
                                    [no_transacciones_ilicitas],
                                    [acepta_monitoreo_info],
                                    [no_listas_restrictivas],
                                    [correo_reportar_incidentes]
                                )
                                VALUES
                                (
                                    @id_formulario,
                                    @sometida_sagrilaft,
                                    @sometida_otro_sistema,
                                    @adhesion_politicas_samsung,
                                    @no_invest_sancion_laftfpadm,
                                    @no_transacciones_ilicitas,
                                    @acepta_monitoreo_info,
                                    @no_listas_restrictivas,
                                    @correo_reportar_incidentes
                                );";

                var parametros = new List<SqlParameter>()
                {
                    new SqlParameter("@id_formulario", SqlDbType.Int) { Value = objRegistro.IdFormulario },
                    new SqlParameter("@sometida_sagrilaft", SqlDbType.Bit) { Value = objRegistro.sometida_sagrilaft },
                    new SqlParameter("@sometida_otro_sistema", SqlDbType.Bit) { Value = objRegistro.sometida_otro_sistema },
                    new SqlParameter("@adhesion_politicas_samsung", SqlDbType.Bit) { Value = objRegistro.adhesion_politicas_samsung },
                    new SqlParameter("@no_invest_sancion_laftfpadm", SqlDbType.Bit) { Value = objRegistro.no_invest_sancion_laftfpadm },
                    new SqlParameter("@no_transacciones_ilicitas", SqlDbType.Bit) { Value = objRegistro.no_transacciones_ilicitas },
                    new SqlParameter("@acepta_monitoreo_info", SqlDbType.Bit) { Value = objRegistro.acepta_monitoreo_info },
                    new SqlParameter("@no_listas_restrictivas", SqlDbType.Bit) { Value = objRegistro.no_listas_restrictivas },

                    new SqlParameter("@correo_reportar_incidentes", SqlDbType.VarChar, 200)
                    {
                        Value = (object) objRegistro.correo_reportar_incidentes ?? DBNull.Value
                    }
                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();

                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("Error al insertar registro en CumplimientoNormativo", ex);
            }
        }

        private bool EditaCumpliomientoNormativo(CumplimientoNormativoDto objRegistro)
        {
            bool respuesta = false;
            string strConsulta = string.Empty;

            try
            {
                strConsulta = @"UPDATE [dbo].[CumplimientoNormativo]
                                SET
                                    [sometida_sagrilaft] = @sometida_sagrilaft,
                                    [sometida_otro_sistema] = @sometida_otro_sistema,
                                    [adhesion_politicas_samsung] = @adhesion_politicas_samsung,
                                    [no_invest_sancion_laftfpadm] = @no_invest_sancion_laftfpadm,
                                    [no_transacciones_ilicitas] = @no_transacciones_ilicitas,
                                    [acepta_monitoreo_info] = @acepta_monitoreo_info,
                                    [no_listas_restrictivas] = @no_listas_restrictivas,
                                    [correo_reportar_incidentes] = @correo_reportar_incidentes
                                WHERE
                                    [id] = @id
                                    AND [id_formulario] = @id_formulario
                            ";

                List<SqlParameter> parametros = new List<SqlParameter>()
                {
                    new SqlParameter("@id", SqlDbType.Int) { Value = objRegistro.Id },
                    new SqlParameter("@id_formulario", SqlDbType.Int) { Value = objRegistro.IdFormulario },
                    new SqlParameter("@sometida_sagrilaft", SqlDbType.Bit) { Value = objRegistro.sometida_sagrilaft },
                    new SqlParameter("@sometida_otro_sistema", SqlDbType.Bit) { Value = objRegistro.sometida_otro_sistema },
                    new SqlParameter("@adhesion_politicas_samsung", SqlDbType.Bit) { Value = objRegistro.adhesion_politicas_samsung },
                    new SqlParameter("@no_invest_sancion_laftfpadm", SqlDbType.Bit) { Value = objRegistro.no_invest_sancion_laftfpadm },
                    new SqlParameter("@no_transacciones_ilicitas", SqlDbType.Bit) { Value = objRegistro.no_transacciones_ilicitas },
                    new SqlParameter("@acepta_monitoreo_info", SqlDbType.Bit) { Value = objRegistro.acepta_monitoreo_info },
                    new SqlParameter("@no_listas_restrictivas", SqlDbType.Bit) { Value = objRegistro.no_listas_restrictivas },
                    new SqlParameter("@correo_reportar_incidentes", SqlDbType.VarChar, 200)
                    {
                        Value = (object) objRegistro.correo_reportar_incidentes ?? DBNull.Value
                    }
                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("Error al editar el registro de CumplimientoNormativo", ex);
            }

            return respuesta;
        }

        public async Task<bool> GuardaDespachoMercancia(DespachoMercanciaDto objRegistro)
        {
            bool Respuesta = false;
            try
            {
                if (existeDespachoMercancia(objRegistro.Id, objRegistro.IdFormulario) || (objRegistro.Id != 0))
                {
                    Respuesta = EditaDespachoMercancia(objRegistro);
                }
                else
                {
                    Respuesta = GuardaDespachoMerc(objRegistro);
                }
            }
            catch (Exception ex)
            {
                Respuesta = false;
                throw new InvalidOperationException("error al CreaR DatosContacto");
            }
            return Respuesta;

        }
        private bool existeDespachoMercancia(int Id, int IdFormulario)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[DespachoMercancia] where Id={0} and IdFormulario={1}", Id, IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        private void EliminarDespachoMercancia(int IdFormulario)
        {
            try
            {
                string querydelete = "Delete from [dbo].[DespachoMercancia] where IdFormulario=@IdFormulario";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(querydelete, parametros);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
            }

        }

        private bool GuardaDespachoMerc(DespachoMercanciaDto objRegistro)
        {
            try
            {
                string query = "insert into [dbo].[DespachoMercancia] " +
                         "VALUES (@IdFormulario, @DireccionDespacho, @Pais, @Cuidad, @CodigoPostalEnvio, @Telefono, @EmailCorporativo)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@DireccionDespacho ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.DireccionDespacho },
                    new SqlParameter() { ParameterName = "@Pais ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Pais },
                    new SqlParameter() { ParameterName = "@Cuidad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Cuidad },
                    new SqlParameter() { ParameterName = "@CodigoPostalEnvio ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CodigoPostalEnvio },
                     new SqlParameter() { ParameterName = "@Telefono ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Telefono },
                    new SqlParameter() { ParameterName = "@EmailCorporativo ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.EmailCorporativo },
                };
                cDataBase.conectar();


                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();
                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");

            }

        }

        private bool EditaDespachoMercancia(DespachoMercanciaDto objRegistro)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[DespachoMercancia] " +
               "SET [DireccionDespacho] = @DireccionDespacho, " +
               "[Pais] = @Pais, " +
               "[Cuidad] = @Cuidad, " +
               "[CodigoPostalEnvio] = @CodigoPostalEnvio, " +
               "[Telefono] = @Telefono, " +
               "[EmailCorporativo] = @EmailCorporativo " +
               "WHERE [Id] = @Id and [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                    new SqlParameter() { ParameterName = "@Id ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Id },
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@DireccionDespacho ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.DireccionDespacho },
                    new SqlParameter() { ParameterName = "@Pais ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Pais },
                    new SqlParameter() { ParameterName = "@Cuidad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Cuidad },
                    new SqlParameter() { ParameterName = "@CodigoPostalEnvio ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CodigoPostalEnvio },
                     new SqlParameter() { ParameterName = "@Telefono ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Telefono },
                    new SqlParameter() { ParameterName = "@EmailCorporativo ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.EmailCorporativo },

                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el Cumplimiento");
            }
            return respuesta;


        }

        public async Task<DespachoMercanciaDto> ConsulataDespachoMercancia(int IdFormulario)
        {

            string Consulta = string.Empty;
            DespachoMercanciaDto objeto = new DespachoMercanciaDto();
            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[DireccionDespacho],[Pais],[Cuidad] ,[CodigoPostalEnvio] ,[Telefono], [EmailCorporativo] FROM [dbo].[DespachoMercancia] where IdFormulario={0}", IdFormulario);
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                objeto.IdFormulario = IdFormulario;
                objeto.DireccionDespacho = dtInformacion.Rows[0]["DireccionDespacho"].ToString();
                objeto.Pais = Convert.ToInt32(dtInformacion.Rows[0]["Pais"]);
                objeto.Cuidad = dtInformacion.Rows[0]["Cuidad"].ToString();
                objeto.CodigoPostalEnvio = dtInformacion.Rows[0]["CodigoPostalEnvio"].ToString();
                objeto.Telefono = dtInformacion.Rows[0]["Telefono"].ToString();
                objeto.EmailCorporativo = dtInformacion.Rows[0]["EmailCorporativo"].ToString();

                return objeto;
            }

            return null;

        }

        public async Task<bool> GuardaInformacionRepresentantesLegales(int IdFormulario, object objrepresetantes)
        {
            bool Respuesta = false;
            try
            {
                if (ExisteInformacionRepresentantes(IdFormulario))
                {
                    Respuesta = EditaRepresentanteLegal(IdFormulario, objrepresetantes);
                }
                else
                {
                    Respuesta = GuardaRepresentantes(IdFormulario, objrepresetantes);
                }
            }
            catch (Exception ex)
            {
                Respuesta = false;
                throw new InvalidOperationException("error al CreaR DatosContacto");
            }
            return Respuesta;


        }

        public bool GuardaRepresentantes(int IdFormulario, object objrepresetantes)
        {

            try
            {
                string query = "insert into [dbo].[RepresentanteLegal] " +
                         "VALUES (@IdFormulario, @JsonRepresentanteLegal)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                    new SqlParameter() { ParameterName = "@JsonRepresentanteLegal ", SqlDbType = SqlDbType.NVarChar, Value =  objrepresetantes.ToString() },

                };
                cDataBase.conectar();


                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();
                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");

            }

        }


        private bool EditaRepresentanteLegal(int IdFormulario, object objrepresetantes)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[RepresentanteLegal] " +
               "SET [JsonRepresentanteLegal] = @JsonRepresentanteLegal " +
               "WHERE [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                    new SqlParameter() { ParameterName = "@JsonRepresentanteLegal ", SqlDbType = SqlDbType.NVarChar, Value =  objrepresetantes.ToString() },

                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el Cumplimiento");
            }
            return respuesta;


        }

        private bool ExisteInformacionRepresentantes(int IdFormulario)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[RepresentanteLegal] where IdFormulario={0}", IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<object> ConsultaInfoRepresentanteslegales(int IdFormulario)

        {
            object objetojson;
            string Consulta = string.Empty;


            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[JsonRepresentanteLegal] FROM [dbo].[RepresentanteLegal] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objetojson = dtInformacion.Rows[0]["JsonRepresentanteLegal"];
                return objetojson;
            }

            return null;

        }


        public async Task<bool> GuardaInformacionAccionistas(int IdFormulario, object objrepresetantes)
        {
            bool Respuesta = false;
            try
            {
                if (ExisteInformacionAccionistas(IdFormulario))
                {
                    Respuesta = EditaAccionistas(IdFormulario, objrepresetantes);
                }
                else
                {
                    Respuesta = GuardaAccionistas(IdFormulario, objrepresetantes);
                }
            }
            catch (Exception ex)
            {
                Respuesta = false;
                throw new InvalidOperationException("error al Crea accionista");
            }
            return Respuesta;


        }//GuardaInformacionAccionistas

        private bool ExisteInformacionAccionistas(int IdFormulario)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[Accionistas] where IdFormulario={0}", IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool EditaAccionistas(int IdFormulario, object objacionista)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[Accionistas] " +
               "SET [JsonAccionistas] = @JsonAccionistas  " +

               "WHERE [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                    new SqlParameter() { ParameterName = "@JsonAccionistas ", SqlDbType = SqlDbType.NVarChar, Value =  objacionista.ToString() },

                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el accionista");
            }
            return respuesta;


        }

        public bool GuardaAccionistas(int IdFormulario, object objacionista)
        {

            try
            {
                string query = "insert into [dbo].[Accionistas] " +
                         "VALUES (@IdFormulario, @JsonAccionistas)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                    new SqlParameter() { ParameterName = "@JsonAccionistas ", SqlDbType = SqlDbType.NVarChar, Value =  objacionista.ToString() },

                };
                cDataBase.conectar();


                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();
                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el accionista");

            }

        }

        public async Task<object> ConsultaInfoAccionistas(int IdFormulario)

        {

            string Consulta = string.Empty;
            object objetojson = null;

            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[JsonAccionistas] FROM [dbo].[Accionistas] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objetojson = dtInformacion.Rows[0]["JsonAccionistas"];

                return objetojson;
            }

            return null;

        }


        public async Task<bool> GuardaInformacionJuntaDirectiva(int IdFormulario, object objjuntadirectiva)
        {
            bool Respuesta = false;
            try
            {
                if (ExisteInformacionJuntaDirectiva(IdFormulario))
                {
                    Respuesta = EditaJuntaDirectiva(IdFormulario, objjuntadirectiva);
                }
                else
                {
                    Respuesta = GuardaJuntaDirectiva(IdFormulario, objjuntadirectiva);
                }
            }
            catch (Exception ex)
            {
                Respuesta = false;
                throw new InvalidOperationException("error al Crea accionista");
            }
            return Respuesta;


        }//GuardaInformacionAccionistas

        private bool ExisteInformacionJuntaDirectiva(int IdFormulario)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[JuntaDirectiva] where IdFormulario={0}", IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool EditaJuntaDirectiva(int IdFormulario, object objJuntaDirectiva)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[JuntaDirectiva] " +
               "SET [JsonJuntaDirectiva] = @JsonJuntaDirectiva " +
               "WHERE [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                    new SqlParameter() { ParameterName = "@JsonJuntaDirectiva ", SqlDbType = SqlDbType.NVarChar, Value =  objJuntaDirectiva.ToString() },
                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el accionista");
            }
            return respuesta;


        }

        public bool GuardaJuntaDirectiva(int IdFormulario, object objJuntadirectiva)
        {

            try
            {
                string query = "insert into [dbo].[JuntaDirectiva] " +
                         "VALUES (@IdFormulario, @JsonJuntaDirectiva)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                    new SqlParameter() { ParameterName = "@JsonJuntaDirectiva ", SqlDbType = SqlDbType.NVarChar, Value =  objJuntadirectiva.ToString() },

                };
                cDataBase.conectar();


                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();
                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el accionista");

            }

        }

        public async Task<object> ConsultaInfoJuntaDirectiva(int IdFormulario)

        {

            string Consulta = string.Empty;
            object objetojson;

            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[JsonJuntaDirectiva] FROM [dbo].[JuntaDirectiva] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objetojson = dtInformacion.Rows[0]["JsonJuntaDirectiva"];
                return objetojson;
            }

            return null;

        }

        public async Task<bool> GuardaInfoAdjuntos(ArchivoDto objAdjunto)
        {
            bool respuesta = false;

            try
            {
                if (objAdjunto == null)
                {
                    throw new ArgumentNullException(nameof(objAdjunto));
                }

                // Usar consulta parametrizada para evitar SQL injection y problemas con caracteres especiales
                string strConsulta = @"INSERT INTO [dbo].[AdjuntoFormulario] 
                                      ([NombreArchivo], [Extencion], [Peso], [Ubicacion], [Key], [IdFormulario]) 
                                      VALUES 
                                      (@NombreArchivo, @Extension, @Peso, @Ubicacion, @Key, @IdFormulario)";

                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("@NombreArchivo", SqlDbType.VarChar) { Value = objAdjunto.NombreArchivo ?? "" },
                    new SqlParameter("@Extension", SqlDbType.VarChar) { Value = objAdjunto.extencion ?? "" },
                    new SqlParameter("@Peso", SqlDbType.VarChar) { Value = objAdjunto.peso ?? "" },
                    new SqlParameter("@Ubicacion", SqlDbType.VarChar) { Value = objAdjunto.Ubicacion ?? "" },
                    new SqlParameter("@Key", SqlDbType.VarChar) { Value = objAdjunto.Key ?? "" },
                    new SqlParameter("@IdFormulario", SqlDbType.Int) { Value = objAdjunto.IdFormulario }
                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException($"Error al guardar archivo en BD: {ex.Message}", ex);
            }

            return respuesta;
        }


        public async Task<ArchivoDto> ConsultaInfoArchivo(int IdFormualrio, string Key)
        {

            ArchivoDto archivo = new ArchivoDto();
            DataTable dtInformacion = new DataTable();
            try
            {

                string strConsulta = string.Format("SELECT [Id], [NombreArchivo],[Extencion],[Peso] ,[Ubicacion],[Key],[IdFormulario]  FROM [dbo].[AdjuntoFormulario] where [IdFormulario] = {0} and [Key] ='{1}'", IdFormualrio, Key);

                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(strConsulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al consultar archivo");
            }

            if (dtInformacion.Rows.Count > 0)
            {

                archivo.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                archivo.NombreArchivo = dtInformacion.Rows[0]["NombreArchivo"].ToString();
                archivo.extencion = dtInformacion.Rows[0]["Extencion"].ToString();
                archivo.peso = dtInformacion.Rows[0]["Peso"].ToString();
                archivo.Ubicacion = dtInformacion.Rows[0]["Ubicacion"].ToString();
                archivo.Key = dtInformacion.Rows[0]["Key"].ToString();
                archivo.IdFormulario = Convert.ToInt32(dtInformacion.Rows[0]["IdFormulario"]);
                return archivo;
            }

            return null;
        }



        public async Task<List<ArchivoDto>> ConsultaInfoArchivoCargados(int IdFormualrio)
        {

            List<ArchivoDto> lstarchivo = new List<ArchivoDto>();
            DataTable dtInformacion = new DataTable();
            try
            {

                string strConsulta = string.Format("SELECT [Id], [NombreArchivo],[Extencion],[Peso] ,[Ubicacion],[Key],[IdFormulario]  FROM [dbo].[AdjuntoFormulario] where [IdFormulario] = {0}", IdFormualrio);

                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(strConsulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el ususario");
            }

            if (dtInformacion.Rows.Count > 0)
            {

                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    ArchivoDto archivo = new ArchivoDto();
                    archivo.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    archivo.NombreArchivo = dtInformacion.Rows[rows]["NombreArchivo"].ToString();
                    archivo.extencion = dtInformacion.Rows[rows]["Extencion"].ToString();
                    archivo.peso = dtInformacion.Rows[rows]["Peso"].ToString();
                    archivo.Ubicacion = dtInformacion.Rows[rows]["Ubicacion"].ToString();
                    archivo.Key = dtInformacion.Rows[rows]["Key"].ToString();
                    archivo.IdFormulario = Convert.ToInt32(dtInformacion.Rows[rows]["IdFormulario"]);
                    lstarchivo.Add(archivo);
                }
                return lstarchivo;

            }

            return null;
        }


        public async Task<bool> EliminaArchivoBasedatos(int idArchivo)
        {
            string strConsulta = string.Empty;
            bool respuesta = false;
            try
            {
                strConsulta = string.Format("delete from [dbo].[AdjuntoFormulario] where Id={0}", idArchivo);


                cDataBase.conectar();
                cDataBase.ejecutarQuery(strConsulta);
                respuesta = true;
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el ususario");
            }

            return respuesta;
        }

        public async Task<bool> GuardaMotivoRechazoFormulario(RechazoFormularioDto objRechazo, int IdEstado, int IdUsuario)
        {


            try
            {
                string query = "insert into [dbo].[RechazoFormulario] " +
                         "VALUES (@IdFormulario,@MotivoRechazo,GETDATE(),@IdUsuarioRechaza)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRechazo.IdFormulario },
                    new SqlParameter() { ParameterName = "@MotivoRechazo ", SqlDbType = SqlDbType.NVarChar, Value =  objRechazo.MotivoRechazo.ToString() },
                     new SqlParameter() { ParameterName = "@IdUsuarioRechaza ", SqlDbType = SqlDbType.Int, Value =  IdUsuario },

                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);


                string query2 = "Update [dbo].[FormularioClienteProveedores] " +
                    "Set IdEstado=@IdEstado where Id=@IdFormulario";
                List<SqlParameter> parametros2 = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRechazo.IdFormulario },
                     new SqlParameter() { ParameterName = "@IdEstado ", SqlDbType = SqlDbType.Int, Value =  IdEstado },

                };
                cDataBase.EjecutarSPParametrosSinRetornonuew(query2, parametros2);



                cDataBase.desconectar();
                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el accionista");

            }


        }


        public async Task<RechazoFormularioDto> MuestraMotivoRechazo(int IdFormulario)
        {
            string strConsulta = string.Empty;
            DataTable dtInformacion = new DataTable();
            RechazoFormularioDto objRechazo = new RechazoFormularioDto();
            try
            {
                strConsulta = string.Format("select Id,IdFormulario,MotivoRechazo,FechaRechazo from [dbo].[RechazoFormulario] where IdFormulario={0} order by Id desc", IdFormulario);
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(strConsulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el ususario");
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objRechazo.IdFormulario = IdFormulario;
                objRechazo.MotivoRechazo = dtInformacion.Rows[0]["MotivoRechazo"].ToString();
                objRechazo.FechaRechazo = dtInformacion.Rows[0]["FechaRechazo"].ToString();

                return objRechazo;
            }
            else
            {
                return null;
            }
            return null;

        }

        public async Task GuardarConsultaInspektor(InformacionInspektorDto obj)
        {
            try
            {
                string query = "INSERT INTO [dbo].[inspektor_respuesta_ws] ([IdFormulario],[Tipo_Tercero],[Tipo_Identificacion],[Numero_Identificacion],[Nombre],[Numero_Consulta],[Coincidencias],[Fecha_Consulta]) " +
                         "VALUES (@IdFormulario,@Tipo_Tercero,@Tipo_Identificacion,@Numero_Identificacion,@Nombre,@Numero_Consulta,@Coincidencias,GETDATE())";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  obj.IdFomulario },
                    new SqlParameter() { ParameterName = "@Tipo_Tercero ", SqlDbType = SqlDbType.NVarChar, Value =  obj.Tipo_Tercero.ToString() },
                    new SqlParameter() { ParameterName = "@Tipo_Identificacion ", SqlDbType = SqlDbType.Int, Value =  obj.Tipo_Identificacion.ToString() },
                    new SqlParameter() { ParameterName = "@Numero_Identificacion ", SqlDbType = SqlDbType.NVarChar, Value =  obj.Numero_Identificacion.ToString() },
                    new SqlParameter() { ParameterName = "@Nombre ", SqlDbType = SqlDbType.NVarChar, Value =  obj.Nombre.ToString() },
                    new SqlParameter() { ParameterName = "@Numero_Consulta ", SqlDbType = SqlDbType.NVarChar, Value =  obj.Numero_Consulta.ToString() },
                    new SqlParameter() { ParameterName = "@Coincidencias ", SqlDbType = SqlDbType.NVarChar, Value =  obj.Coincidencias.ToString() },

                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el accionista");
            }

        }

        public async Task<List<InformacionInspektorDto>> ListaResultadosInspektor(int IdFormulario)
        {
            List<InformacionInspektorDto> ListaResultados = new List<InformacionInspektorDto>();
            DataTable dtInformacion = ConsultaListaResultadosFomrualrio(IdFormulario);

            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    InformacionInspektorDto objRespuesta = new InformacionInspektorDto();
                    objRespuesta.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objRespuesta.IdFomulario = Convert.ToInt32(dtInformacion.Rows[rows]["IdFormulario"]);
                    objRespuesta.Tipo_Tercero = dtInformacion.Rows[rows]["Tipo_Tercero"].ToString();
                    objRespuesta.Tipo_Identificacion = Convert.ToInt32(dtInformacion.Rows[rows]["Tipo_Identificacion"]);
                    objRespuesta.TipoIdentificacion = dtInformacion.Rows[rows]["TipoDocumento"].ToString();
                    objRespuesta.Numero_Identificacion = dtInformacion.Rows[rows]["Numero_Identificacion"].ToString();
                    objRespuesta.Nombre = dtInformacion.Rows[rows]["Nombre"].ToString();
                    objRespuesta.Numero_Consulta = dtInformacion.Rows[rows]["Numero_Consulta"].ToString();
                    objRespuesta.Coincidencias = dtInformacion.Rows[rows]["Coincidencias"].ToString();
                    objRespuesta.Fecha_Consulta = dtInformacion.Rows[rows]["Fecha_Consulta"].ToString();

                    ListaResultados.Add(objRespuesta);
                }
            }

            return ListaResultados;

        }

        private DataTable ConsultaListaResultadosFomrualrio(int IdFormulario)
        {


            string Consulta = string.Empty;

            Consulta = string.Format("SELECT a.[Id],a.[IdFormulario],a.[Tipo_Tercero],a.[Tipo_Identificacion],b.Nombre_es as TipoDocumento ,a.[Numero_Identificacion],a.[Nombre],a.[Numero_Consulta],a.[Coincidencias],a.[Fecha_Consulta]  FROM .[dbo].[inspektor_respuesta_ws] as a inner join [dbo].[TipoDocumentos] as b on a.Tipo_Identificacion=b.Id where a.IdFormulario ={0}", IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }


        public async Task<List<string>> CorreosControlInterno()//cambiaraqui
        {
            List<string> ListaCorreos = new List<string>();
            DataTable CorreoControlInterno = ConsultaCorreosControlInterno();
            if (CorreoControlInterno.Rows.Count > 0)
            {
                for (int rows = 0; rows < CorreoControlInterno.Rows.Count; rows++)
                {
                    ListaCorreos.Add(CorreoControlInterno.Rows[rows]["Email"].ToString());
                }
            }
            return ListaCorreos;
        }

        private DataTable ConsultaCorreosControlInterno()
        {
            string Consulta = string.Empty;
            Consulta = string.Format("select Email from  [dbo].[tbl_Usuarios] where TipoUsuario=4");
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }


        public async Task<List<string>> CorreosOficialCumplimineto()
        {
            List<string> ListaCorreos = new List<string>();
            DataTable CorreoControlInterno = ConsultaCorreosOficialCumplimiento();
            if (CorreoControlInterno.Rows.Count > 0)
            {
                ListaCorreos.Add(CorreoControlInterno.Rows[0]["Email"].ToString());
            }
            return ListaCorreos;
        }

        private DataTable ConsultaCorreosOficialCumplimiento()
        {
            string Consulta = string.Empty;
            Consulta = string.Format("select Email from  [dbo].[tbl_Usuarios] where TipoUsuario=5");
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }



        public async Task<List<string>> CorreosEnvioFormualrio(int IdFormulario)
        {
            List<string> ListaCorreos = new List<string>();

            DataTable CorreosContabilidad = ConsultaCorreosContabilidad();

            if (CorreosContabilidad.Rows.Count > 0)
            {
                foreach (DataRow fila in CorreosContabilidad.Rows)
                {
                    ListaCorreos.Add(fila["Email"].ToString());
                }

            }

            FormularioDto objform = new FormularioDto();

            objform = await InfoFormulario(IdFormulario);

            int idCompradorVendedor = await DevulveIdCompradorVendedor(objform.IdUsuario);

            ListaCorreos.Add(await DevuelveCorreoCompradorVendedor(idCompradorVendedor));
            return ListaCorreos;

        }

        private async Task<int> DevulveIdCompradorVendedor(int IdUsuarioClienteProveedor)
        {

            int Idusuario = 0;

            string Consulta = string.Empty;
            Consulta = string.Format("select * from [dbo].[tbl_Usuarios] where Id={0}", IdUsuarioClienteProveedor);
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                Idusuario = Convert.ToInt32(dtInformacion.Rows[0]["IdCompradorVendedor"]);

            }

            return Idusuario;
        }




        public async Task<List<string>> CorreosCorreccionFormulario(int IdFormulario)
        {
            List<string> ListaCorreos = new List<string>();


            DataTable CorreoProveedorcliente = ConsultaCorreosUsuario(IdFormulario);
            if (CorreoProveedorcliente.Rows.Count > 0)
            {
                ListaCorreos.Add(CorreoProveedorcliente.Rows[0]["Email"].ToString());
            }

            FormularioDto objform = new FormularioDto();

            objform = await InfoFormulario(IdFormulario);

            int idCompradorVendedor = await DevulveIdCompradorVendedor(objform.IdUsuario);

            ListaCorreos.Add(await DevuelveCorreoCompradorVendedor(idCompradorVendedor));
            return ListaCorreos;

        }

        private DataTable ConsultaCorreosContabilidad()
        {
            string Consulta = string.Empty;
            Consulta = string.Format("select * from [dbo].[tbl_Usuarios] where TipoUsuario=3");
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }


        private DataTable ConsultaCorreosUsuario(int IdFormulario)
        {
            string Consulta = string.Empty;
            Consulta = string.Format("select a.* from [dbo].[tbl_Usuarios] as a inner join [dbo].[FormularioClienteProveedores] as b on (b.IdUsuario=a.Id) where b.Id = {0}", IdFormulario);
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }


        public async Task<string> DevuelveCorreoCompradorVendedor(int IdUsuario)
        {
            string CorreoCompradorVendedor = string.Empty;

            DataTable dtInformacion = ConsultaCorreoVendedorComprador(IdUsuario);

            if (dtInformacion.Rows.Count > 0)
            {
                CorreoCompradorVendedor = dtInformacion.Rows[0]["Email"].ToString();
            }

            return CorreoCompradorVendedor;

        }


        private DataTable ConsultaCorreoVendedorComprador(int idUsuario)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string consonsulta = string.Format("select Email from  [dbo].[tbl_Usuarios] where Id={0}", idUsuario);
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(consonsulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }



        public async Task<FormularioDto> InfoFormulario(int IdFormualrio)
        {

            FormularioDto objformualrio = new FormularioDto();
            DataTable dtInformacion = ConsultaInfoFormualrio(IdFormualrio);


            objformualrio.Id = IdFormualrio; ;
            objformualrio.IdUsuario = Convert.ToInt32(dtInformacion.Rows[0]["IdUsuario"]);
            objformualrio.NombreUsuario = dtInformacion.Rows[0]["NombreUsuario"].ToString();
            objformualrio.IdEstado = Convert.ToInt32(dtInformacion.Rows[0]["IdEstado"]);
            objformualrio.Estado = dtInformacion.Rows[0]["Estado"].ToString();
            objformualrio.FechaFormulario = dtInformacion.Rows[0]["FechaFormulario"].ToString();


            return objformualrio;


        }

        private DataTable ConsultaInfoFormualrio(int IdFormulario)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string consonsulta = string.Format("select FPC.Id, FPC.IdUsuario, CONCAT(usu.Nombre,' ' , usu.Apellidos) as NombreUsuario, FPC.IdEstado,EF.Nombre_ES as Estado,FPC.FechaFormulario from [dbo].[FormularioClienteProveedores] as FPC inner join [dbo].[tbl_Usuarios] as usu ON (FPC.IdUsuario=usu.Id) inner join [dbo].[EstadoFormulario] as EF ON (EF.Id=FPC.IdEstado) where FPC.Id={0} order by FPC.Id desc", IdFormulario);
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(consonsulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }



        public async Task<bool> GuardarInformacionOEA(FormularioModelDTO objRegistro, int IdUsuario)
        {
            bool Respeusta = false;

            if (ExisteDatosOEA(objRegistro.IdFormulario))
            {
                Respeusta = EditaInformacionOEA(objRegistro);

            }
            else
            {
                Respeusta = GuardaInformacionOEA(objRegistro, IdUsuario);
            }

            return Respeusta;
        }

        private bool ExisteDatosOEA(int IdFormulario)
        {
            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[InformacionOEA] where IdFormulario={0}", IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool EditaInformacionOEA(FormularioModelDTO objRegistro)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[InformacionOEA] " +
               "SET [Uen] = @Uen, " +
               "[ResponsableVenta] = @ResponsableVenta, " +
               "[CorreoElectronico] = @CorreoElectronico, " +
               "[ResponsableCartera] = @ResponsableCartera, " +
               "[ResponsableTecnico] = @ResponsableTecnico, " +
               "[Moneda] = @Moneda, " +
               "[FormaPago] = @FormaPago, " +
               "[NumeroDias]= @NumeroDias, " +
               "[CadenaLogistica] = @CadenaLogistica, " +
               "[ListasRiesgo] = @ListasRiesgo, " +
               "[SustanciasNarcoticos] = @SustanciasNarcoticos, " +
               "[Certificaciones] = @Certificaciones, " +
               "[ProveedorCadenaLogistica] = @ProveedorCadenaLogistica, " +
               "[RiesgoPais] = @RiesgoPais, " +
               "[AntiguedadEmpresa] = @AntiguedadEmpresa, " +
               "[RiesgoSeguridad] = @RiesgoSeguridad, " +
               "[Valoracion] = @Valoracion, " +
               "[ListasRiesgoCliente] = @ListasRiesgoCliente, " +
               "[TipoNegociacion] = @TipoNegociacion, " +
               "[VistoBuenoAseguradora] = @VistoBuenoAseguradora, " +
               "[RiesgoPaisCliente] = @RiesgoPaisCliente, " +
               "[CertificacionesInstitucionalidad] = @CertificacionesInstitucionalidad, " +
               "[RiesgoSeguridadCliente] = @RiesgoSeguridadCliente, " +
               "[ValoracionCliente] = @ValoracionCliente, " +
               "[SegmentacionRiesgo] = @SegmentacionRiesgo " +
               "WHERE [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@Uen ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Uen },
                    new SqlParameter() { ParameterName = "@ResponsableVenta ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ResponsableVenta },
                     new SqlParameter() { ParameterName = "@CorreoElectronico ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronico },
                      new SqlParameter() { ParameterName = "@ResponsableCartera ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ResponsableCartera },
                       new SqlParameter() { ParameterName = "@ResponsableTecnico ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ResponsableTecnico },
                       new SqlParameter() { ParameterName = "@Moneda ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Moneda },
                       new SqlParameter() { ParameterName = "@FormaPago ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.FormaPago },
                       new SqlParameter() { ParameterName = "@NumeroDias ", SqlDbType = SqlDbType.Int, Value =  objRegistro.NumeroDias },
                       new SqlParameter() { ParameterName = "@CadenaLogistica ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CadenaLogistica },
                       new SqlParameter() { ParameterName = "@ListasRiesgo ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ListasRiesgo },
                       new SqlParameter() { ParameterName = "@SustanciasNarcoticos ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.SustanciasNarcoticos },
                       new SqlParameter() { ParameterName = "@Certificaciones ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Certificaciones },
                       new SqlParameter() { ParameterName = "@ProveedorCadenaLogistica ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ProveedorCadenaLogistica },
                       new SqlParameter() { ParameterName = "@RiesgoPais ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.RiesgoPais },
                       new SqlParameter() { ParameterName = "@AntiguedadEmpresa", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.AntiguedadEmpresa },
                       new SqlParameter() { ParameterName = "@RiesgoSeguridad", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.RiesgoSeguridad },
                        new SqlParameter() { ParameterName = "@Valoracion", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Valoracion },
                        new SqlParameter() { ParameterName = "@ListasRiesgoCliente", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ListasRiesgoCliente },
                        new SqlParameter() { ParameterName = "@TipoNegociacion", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.TipoNegociacion },
                        new SqlParameter() { ParameterName = "@VistoBuenoAseguradora", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.VistoBuenoAseguradora },
                        new SqlParameter() { ParameterName = "@RiesgoPaisCliente", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.RiesgoPaisCliente },
                        new SqlParameter() { ParameterName = "@CertificacionesInstitucionalidad", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CertificacionesInstitucionalidad },
                        new SqlParameter() { ParameterName = "@RiesgoSeguridadCliente", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.RiesgoSeguridadCliente },
                        new SqlParameter() { ParameterName = "@ValoracionCliente", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ValoracionCliente },
                        new SqlParameter() { ParameterName = "@SegmentacionRiesgo", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.SegmentacionRiesgo },
                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el DatosGenerales");
            }
            return respuesta;


        }

        private bool GuardaInformacionOEA(FormularioModelDTO objRegistro, int IdUsuario)
        {
            bool respuesta = false;
            try
            {

                string query = "insert into [dbo].[InformacionOEA] " +
                          "VALUES (@IdFormulario, @Uen, @ResponsableVenta, @CorreoElectronico, @ResponsableCartera, @ResponsableTecnico, @Moneda, @FormaPago, @NumeroDias, @CadenaLogistica, @ListasRiesgo, @SustanciasNarcoticos, @Certificaciones, @ProveedorCadenaLogistica, @RiesgoPais, @AntiguedadEmpresa, @RiesgoSeguridad, @Valoracion, @ListasRiesgoCliente, @TipoNegociacion,@VistoBuenoAseguradora,@RiesgoPaisCliente,@CertificacionesInstitucionalidads,@RiesgoSeguridadCliente,@ValoracionCliente,@IdUsuario,getdate(),@SegmentacionRiesgo)";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                     new SqlParameter() { ParameterName = "@IdUsuario ", SqlDbType = SqlDbType.Int, Value =  IdUsuario },
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@Uen ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Uen },
                    new SqlParameter() { ParameterName = "@ResponsableVenta ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ResponsableVenta },
                     new SqlParameter() { ParameterName = "@CorreoElectronico ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronico },
                      new SqlParameter() { ParameterName = "@ResponsableCartera ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ResponsableCartera },
                       new SqlParameter() { ParameterName = "@ResponsableTecnico ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ResponsableTecnico },
                       new SqlParameter() { ParameterName = "@Moneda ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Moneda },
                       new SqlParameter() { ParameterName = "@FormaPago ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.FormaPago },
                       new SqlParameter() { ParameterName = "@NumeroDias ", SqlDbType = SqlDbType.Int, Value =  objRegistro.NumeroDias },
                       new SqlParameter() { ParameterName = "@CadenaLogistica ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CadenaLogistica },
                       new SqlParameter() { ParameterName = "@ListasRiesgo ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ListasRiesgo },
                       new SqlParameter() { ParameterName = "@SustanciasNarcoticos ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.SustanciasNarcoticos },
                       new SqlParameter() { ParameterName = "@Certificaciones ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Certificaciones },
                       new SqlParameter() { ParameterName = "@ProveedorCadenaLogistica ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ProveedorCadenaLogistica },
                       new SqlParameter() { ParameterName = "@RiesgoPais ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.RiesgoPais },
                       new SqlParameter() { ParameterName = "@AntiguedadEmpresa", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.AntiguedadEmpresa },
                       new SqlParameter() { ParameterName = "@RiesgoSeguridad", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.RiesgoSeguridad },
                        new SqlParameter() { ParameterName = "@Valoracion", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Valoracion },
                        new SqlParameter() { ParameterName = "@ListasRiesgoCliente", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ListasRiesgoCliente },
                        new SqlParameter() { ParameterName = "@TipoNegociacion", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.TipoNegociacion },
                        new SqlParameter() { ParameterName = "@VistoBuenoAseguradora", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.VistoBuenoAseguradora },
                        new SqlParameter() { ParameterName = "@RiesgoPaisCliente", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.RiesgoPaisCliente },
                        new SqlParameter() { ParameterName = "@CertificacionesInstitucionalidads", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CertificacionesInstitucionalidad },
                        new SqlParameter() { ParameterName = "@RiesgoSeguridadCliente", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.RiesgoSeguridadCliente },
                        new SqlParameter() { ParameterName = "@ValoracionCliente", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ValoracionCliente },
                        new SqlParameter() { ParameterName = "@SegmentacionRiesgo", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.SegmentacionRiesgo },

                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                respuesta = true;
                cDataBase.desconectar();



            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear OEA");
                respuesta = false;


            }

            return respuesta;

        }

        public async Task<FormularioModelDTO> ConsultaDatosInformacionOEA(int IdFormulario)

        {

            string Consulta = string.Empty;
            FormularioModelDTO objeto = new FormularioModelDTO();
            Consulta = string.Format("sELECT * FROM [dbo].[InformacionOEA] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objeto.IdFormulario = IdFormulario;
                objeto.Uen = dtInformacion.Rows[0]["Uen"].ToString();
                objeto.ResponsableVenta = dtInformacion.Rows[0]["ResponsableVenta"].ToString();
                objeto.CorreoElectronico = dtInformacion.Rows[0]["CorreoElectronico"].ToString();
                objeto.ResponsableCartera = dtInformacion.Rows[0]["ResponsableCartera"].ToString();
                objeto.ResponsableTecnico = dtInformacion.Rows[0]["ResponsableTecnico"].ToString();
                objeto.Moneda = dtInformacion.Rows[0]["Moneda"].ToString();
                objeto.FormaPago = dtInformacion.Rows[0]["FormaPago"].ToString();
                objeto.NumeroDias = Convert.ToInt32(dtInformacion.Rows[0]["NumeroDias"]);
                objeto.CadenaLogistica = dtInformacion.Rows[0]["CadenaLogistica"].ToString();
                objeto.ListasRiesgo = dtInformacion.Rows[0]["ListasRiesgo"].ToString();
                objeto.SustanciasNarcoticos = dtInformacion.Rows[0]["SustanciasNarcoticos"].ToString();
                objeto.Certificaciones = dtInformacion.Rows[0]["Certificaciones"].ToString();
                objeto.ProveedorCadenaLogistica = dtInformacion.Rows[0]["ProveedorCadenaLogistica"].ToString();
                objeto.RiesgoPais = dtInformacion.Rows[0]["RiesgoPais"].ToString();
                objeto.AntiguedadEmpresa = dtInformacion.Rows[0]["AntiguedadEmpresa"].ToString();
                objeto.RiesgoSeguridad = dtInformacion.Rows[0]["RiesgoSeguridad"].ToString();
                objeto.Valoracion = dtInformacion.Rows[0]["Valoracion"].ToString();
                objeto.ListasRiesgoCliente = dtInformacion.Rows[0]["ListasRiesgoCliente"].ToString();
                objeto.TipoNegociacion = dtInformacion.Rows[0]["TipoNegociacion"].ToString();
                objeto.VistoBuenoAseguradora = dtInformacion.Rows[0]["VistoBuenoAseguradora"].ToString();
                objeto.RiesgoPaisCliente = dtInformacion.Rows[0]["RiesgoPaisCliente"].ToString();
                objeto.CertificacionesInstitucionalidad = dtInformacion.Rows[0]["CertificacionesInstitucionalidad"].ToString();
                objeto.RiesgoSeguridadCliente = dtInformacion.Rows[0]["RiesgoSeguridadCliente"].ToString();
                objeto.ValoracionCliente = dtInformacion.Rows[0]["ValoracionCliente"].ToString();
                objeto.SegmentacionRiesgo = dtInformacion.Rows[0]["SegmentacionRiesgo"].ToString();
                return objeto;
            }

            return null;

        }


        public async Task<UserFormInformationDTO> Userinfo(int IdFormulario)
        {
            DataTable dtInformacion = new DataTable();
            string strConsulta;
            try
            {
                strConsulta = String.Format("SELECT u.id,u.Nombre,u.Apellidos,u.Email,b.Nombre,u.Identificacion as Identificacion,u.ActualizarPass FROM tbl_Usuarios u  inner join [dbo].[tbl_TipoUsuario] b on (u.TipoUsuario=b.Id) inner join [dbo].[FormularioClienteProveedores] AS FCP ON (FCP.IdUsuario=u.Id) where FCP.Id={0} GROUP BY u.Id, u.Nombre, u.Apellidos,u.Email,b.Nombre,u.IdArea,u.ActualizarPass,u.Identificacion", IdFormulario);
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(strConsulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
            }

            if (dtInformacion.Rows.Count > 0)
            {
                UserFormInformationDTO userinfo = new UserFormInformationDTO();
                userinfo.Nombre = dtInformacion.Rows[0]["Nombre"].ToString().Trim();
                userinfo.Apellido = dtInformacion.Rows[0]["Apellidos"].ToString().Trim();
                userinfo.Usuario = dtInformacion.Rows[0]["Email"].ToString().Trim();
                userinfo.CorreoElectronico = dtInformacion.Rows[0]["Email"].ToString().Trim();
                userinfo.Identificacion = dtInformacion.Rows[0]["Identificacion"].ToString().Trim();

                return userinfo;

            }
            else
            {
                return null;
            }

        }


        public async Task<string> DevulveNombrePais(int Pais)
        {
            DataTable dtInformacion = new DataTable();
            string Respuesta = string.Empty;
            string strConsulta;
            try
            {
                strConsulta = String.Format("SELECT [Id],[Nombre_es] ,[Nombre_en] FROM [dbo].[Paises] where Id={0}", Pais);
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(strConsulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
            }

            if (dtInformacion.Rows.Count > 0)
            {

                Respuesta = dtInformacion.Rows[0]["Nombre_es"].ToString();

            }

            return Respuesta;
        }


        public void GuardaPeticionRespuestaErp(PeticionRespuestaERPDTO objRegistro)
        {
            bool respuesta = false;
            try
            {
                string query = "insert into [dbo].[tblConsumoERP] " +
                          "VALUES (@Peticion, @Respuesta, Getdate(), @IdFormulario)";


                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@Peticion ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Peticion.ToString() },
                    new SqlParameter() { ParameterName = "@Respuesta ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Respuesta.ToString() },
                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                respuesta = true;
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();

            }


        }

        public async Task<bool> GuardarDeclaraciones(DeclaracionesDto objRegistro)
        {
            bool Respeusta = false;

            if (ExisteDeclaraciones(objRegistro.Id, objRegistro.IdFormulario) || (objRegistro.Id != 0))
            {
                Respeusta = EditaDeclaraciones(objRegistro);

            }
            else
            {
                Respeusta = GuardaDeclaracionesFist(objRegistro);
            }

            return Respeusta;
        }



        private bool GuardaDeclaracionesFist(DeclaracionesDto objRegistro)
        {
            bool respuesta = false;
            try
            {

                string query = "insert into [dbo].[DeclaracionFormulario] " +
                          "VALUES (@IdFormulario, @NombreRepresentanteFirma, @CorreoRepresentante)";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@NombreRepresentanteFirma ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreRepresentanteFirma },
                    new SqlParameter() { ParameterName = "@CorreoRepresentante ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoRepresentante },
               };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                respuesta = true;
                cDataBase.desconectar();



            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
                respuesta = false;


            }

            return respuesta;

        }


        private bool EditaDeclaraciones(DeclaracionesDto objRegistro)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[DeclaracionFormulario] " +
               "SET [NombreRepresentanteFirma] = @NombreRepresentanteFirma, " +
               "[CorreoRepresentante] = @CorreoRepresentante " +

               "WHERE [Id] = @Id and [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@Id ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Id },
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@NombreRepresentanteFirma ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreRepresentanteFirma },
                    new SqlParameter() { ParameterName = "@CorreoRepresentante ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoRepresentante },
               };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el DatosGenerales");
            }
            return respuesta;


        }



        private bool ExisteDeclaraciones(int Id, int IdFormulario)
        {


            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[DeclaracionFormulario] where Id={0} and IdFormulario={1}", Id, IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public async Task<DeclaracionesDto> ConsultaDeclaraciones(int IdFormulario)

        {

            string Consulta = string.Empty;
            DeclaracionesDto objeto = new DeclaracionesDto();
            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[NombreRepresentanteFirma] ,[CorreoRepresentante] FROM [dbo].[DeclaracionFormulario] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                objeto.IdFormulario = Convert.ToInt32(dtInformacion.Rows[0]["IdFormulario"]);
                objeto.NombreRepresentanteFirma = dtInformacion.Rows[0]["NombreRepresentanteFirma"].ToString();
                objeto.CorreoRepresentante = dtInformacion.Rows[0]["CorreoRepresentante"].ToString();
                return objeto;
            }

            return null;
        }



        public async Task<bool> GuardarInformacionTriburaria(InformacionTributariaDTO objRegistro)
        {
            bool Respeusta = false;

            if (ExisteInformacionTriburaria(objRegistro.Id, objRegistro.IdFormulario) || (objRegistro.Id != 0))
            {
                Respeusta = EditaInformacionTributaria(objRegistro);

            }
            else
            {
                Respeusta = GuardaInformacionTriburaria(objRegistro);
            }

            return Respeusta;
        }



        private bool GuardaInformacionTriburaria(InformacionTributariaDTO objRegistro)
        {
            bool respuesta = false;
            try
            {

                string query = "insert into [dbo].[InformacionTributaria]" +
                          "VALUES (@IdFormulario, @GranContribuyente,@NumResolucionGranContribuyente,@FechaResolucionGranContribuyente,@Autorretenedor,@NumResolucionAutorretenedor,@FechaResolucionAutorretenedor,@ResponsableICA,@MunicipioRetener,@Tarifa,@ResponsableIVA,@AgenteRetenedorIVA,@Sucursal, @RegimenTributario, @Retenciones)";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                    new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@GranContribuyente ", SqlDbType = SqlDbType.Int, Value =  objRegistro.GranContribuyente },
                    new SqlParameter() { ParameterName = "@NumResolucionGranContribuyente ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NumResolucionGranContribuyente },
                    new SqlParameter() { ParameterName = "@FechaResolucionGranContribuyente ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.FechaResolucionGranContribuyente },
                    new SqlParameter() { ParameterName = "@Autorretenedor ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Autorretenedor },

                    new SqlParameter() { ParameterName = "@NumResolucionAutorretenedor ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NumResolucionAutorretenedor },
                    new SqlParameter() { ParameterName = "@FechaResolucionAutorretenedor ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.FechaResolucionAutorretenedor },
                    new SqlParameter() { ParameterName = "@ResponsableICA ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ResponsableICA },
                    new SqlParameter() { ParameterName = "@MunicipioRetener ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.MunicipioRetener },
                    new SqlParameter() { ParameterName = "@Tarifa", SqlDbType = SqlDbType.Int, Value = objRegistro.Tarifa },
                    new SqlParameter() { ParameterName = "@ResponsableIVA ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ResponsableIVA },
                    new SqlParameter() { ParameterName = "@AgenteRetenedorIVA ", SqlDbType = SqlDbType.Int, Value =  objRegistro.AgenteRetenedorIVA },
                    new SqlParameter() { ParameterName = "@Sucursal", SqlDbType = SqlDbType.VarChar, Value = objRegistro.Sucursal ?? (object)DBNull.Value },
                    new SqlParameter() { ParameterName = "@RegimenTributario", SqlDbType = SqlDbType.VarChar, Value = objRegistro.RegimenTributario ?? (object)DBNull.Value },
                    new SqlParameter() { ParameterName = "@Retenciones", SqlDbType = SqlDbType.NVarChar, Value = objRegistro.Retenciones.ToString()},

                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                respuesta = true;
                cDataBase.desconectar();



            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al GuardaInformacionTriburaria");
                respuesta = false;


            }

            return respuesta;

        }


        private bool EditaInformacionTributaria(InformacionTributariaDTO objRegistro)
        {
            bool respuesta = false;
            try
            {
                string strConsulta = @"
        UPDATE [dbo].[InformacionTributaria]
        SET [GranContribuyente] = @GranContribuyente,
            [NumResolucionGranContribuyente] = @NumResolucionGranContribuyente,
            [FechaResolucionGranContribuyente] = @FechaResolucionGranContribuyente,
            [Autorretenedor] = @Autorretenedor,
            [NumResolucionAutorretenedor] = @NumResolucionAutorretenedor,
            [FechaResolucionAutorretenedor] = @FechaResolucionAutorretenedor,
            [ResponsableICA] = @ResponsableICA,
            [MunicipioRetener] = @MunicipioRetener,
            [Tarifa] = @Tarifa,
            [ResponsableIVA] = @ResponsableIVA,
            [AgenteRetenedorIVA] = @AgenteRetenedorIVA,
            [Sucursal] = @Sucursal,
            [RegimenTributario] = @RegimenTributario,
            [Retenciones] = @Retenciones
        WHERE [Id] = @Id AND [IdFormulario]= @IdFormulario";

                // --- extraer JSON crudo de Retenciones ---
                string retencionesJson;
                if (objRegistro.Retenciones is JsonElement je)
                    retencionesJson = je.GetRawText();
                else
                    retencionesJson = JsonConvert.SerializeObject(objRegistro.Retenciones);

                List<SqlParameter> parametros = new List<SqlParameter>()
        {
            new SqlParameter("@Id", SqlDbType.Int){ Value = objRegistro.Id },
            new SqlParameter("@IdFormulario", SqlDbType.Int){ Value = objRegistro.IdFormulario },
            new SqlParameter("@GranContribuyente", SqlDbType.Int){ Value = objRegistro.GranContribuyente },
            new SqlParameter("@NumResolucionGranContribuyente", SqlDbType.VarChar){ Value = objRegistro.NumResolucionGranContribuyente ?? (object)DBNull.Value },
            new SqlParameter("@FechaResolucionGranContribuyente", SqlDbType.VarChar){ Value = objRegistro.FechaResolucionGranContribuyente ?? (object)DBNull.Value },
            new SqlParameter("@Autorretenedor", SqlDbType.Int){ Value = objRegistro.Autorretenedor },
            new SqlParameter("@NumResolucionAutorretenedor", SqlDbType.VarChar){ Value = objRegistro.NumResolucionAutorretenedor ?? (object)DBNull.Value },
            new SqlParameter("@FechaResolucionAutorretenedor", SqlDbType.VarChar){ Value = objRegistro.FechaResolucionAutorretenedor ?? (object)DBNull.Value },
            new SqlParameter("@ResponsableICA", SqlDbType.Int){ Value = objRegistro.ResponsableICA },
            new SqlParameter("@MunicipioRetener", SqlDbType.VarChar){ Value = objRegistro.MunicipioRetener ?? (object)DBNull.Value },
            new SqlParameter("@Tarifa", SqlDbType.Int){ Value = objRegistro.Tarifa },
            new SqlParameter("@ResponsableIVA", SqlDbType.Int){ Value = objRegistro.ResponsableIVA },
            new SqlParameter("@AgenteRetenedorIVA", SqlDbType.Int){ Value = objRegistro.AgenteRetenedorIVA },
            new SqlParameter("@Sucursal", SqlDbType.VarChar){ Value = objRegistro.Sucursal ?? (object)DBNull.Value },
            new SqlParameter("@RegimenTributario", SqlDbType.VarChar){ Value = objRegistro.RegimenTributario ?? (object)DBNull.Value },
            new SqlParameter("@Retenciones", SqlDbType.NVarChar){ Value = retencionesJson ?? (object)DBNull.Value }
        };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();
            }
            catch (Exception)
            {
                cDataBase.desconectar();
                throw;
            }
            return respuesta;
        }



        private bool ExisteInformacionTriburaria(int Id, int IdFormulario)
        {


            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[InformacionTributaria] where Id={0} and IdFormulario={1}", Id, IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public async Task<InformacionTributariaDTO> ConsultaInformacionTributaria(int IdFormulario)

        {

            string Consulta = string.Empty;
            InformacionTributariaDTO objeto = new InformacionTributariaDTO();
            Consulta = string.Format("SELECT [Id],[IdFormulario] ,[GranContribuyente],[NumResolucionGranContribuyente] ,[FechaResolucionGranContribuyente],[Autorretenedor] ,[NumResolucionAutorretenedor],[FechaResolucionAutorretenedor] ,[ResponsableICA] ,[MunicipioRetener] ,[Tarifa] ,[ResponsableIVA] ,[AgenteRetenedorIVA], [Sucursal], [RegimenTributario], [Retenciones] FROM [dbo].[InformacionTributaria] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                objeto.IdFormulario = IdFormulario;
                objeto.GranContribuyente = Convert.ToInt32(dtInformacion.Rows[0]["GranContribuyente"]);
                objeto.NumResolucionGranContribuyente = dtInformacion.Rows[0]["NumResolucionGranContribuyente"].ToString();
                objeto.FechaResolucionGranContribuyente = dtInformacion.Rows[0]["FechaResolucionGranContribuyente"].ToString();
                objeto.Autorretenedor = Convert.ToInt32(dtInformacion.Rows[0]["Autorretenedor"]);
                objeto.NumResolucionAutorretenedor = dtInformacion.Rows[0]["NumResolucionAutorretenedor"].ToString();
                objeto.FechaResolucionAutorretenedor = dtInformacion.Rows[0]["FechaResolucionAutorretenedor"].ToString();
                objeto.ResponsableICA = Convert.ToInt32(dtInformacion.Rows[0]["ResponsableICA"]);
                objeto.MunicipioRetener = dtInformacion.Rows[0]["MunicipioRetener"].ToString();
                objeto.Tarifa = Convert.ToInt32(dtInformacion.Rows[0]["Tarifa"]);
                objeto.ResponsableIVA = Convert.ToInt32(dtInformacion.Rows[0]["ResponsableIVA"]);
                objeto.AgenteRetenedorIVA = Convert.ToInt32(dtInformacion.Rows[0]["AgenteRetenedorIVA"]);
                objeto.Sucursal = dtInformacion.Rows[0]["Sucursal"].ToString();
                objeto.RegimenTributario = dtInformacion.Rows[0]["RegimenTributario"].ToString();
                objeto.Retenciones = dtInformacion.Rows[0]["Retenciones"];



                return objeto;
            }

            return null;

        }

        public async Task<DatosGeneralesReporteDto> ConsultaDatosGeneralesAlertaPaises(int IdFormulario)
        {

            string Consulta = string.Empty;
            string where = String.Format("Where tblDG.IdFormulario={0}", IdFormulario);
            Consulta = string.Format("SELECT tblDG.[Id],tblDG.[IdFormulario],tblDG.[Empresa],tblTSolicitud.Nombre_es TipoSolicitud,tblTerc.Nombre_es ClaseTercero ,tblCatTer.Nombre_es CategoriaTercero,tblDG.[NombreRazonSocial], " +
                "tblTipDocon.Nombre_es TipoIdentificacion ,tblDG.[NumeroIdentificacion],tblDG.[DigitoVerificacion],tblPais.Nombre_es Pais ,tblDG.[Ciudad] ,tblTamTerc.Nombre_es TamanoTercero ,tblActEco.Nombre_es RazonSocial,tblDG.[DireccionPrincipal],tblDG.[CodigoPostal] ,tblDG.[CorreoElectronico],tblDG.[Telefono],Ofactu.Nombre_es ObligadoFacturarElectronicamente,tblDG.[CorreoElectronicoFacturaEletronica],sucOPais.Nombre_es SucursalOtroPais  ,tblDG.[OtroPais], tblDG.[JsonPreguntasPep], tblDG.[EstadoCivil], tblDG.[@ConyugeIdentificacion], tblDG.[@tipoPago], tblDG.[CertBASC], tblDG.[CertOEA], tblDG.[CertCTPAT], tblDG.[CertOtros], tblDG.[CertNinguno] FROM [dbo].[DatosGenerales] as tblDG " +
                "inner join [dbo].[TipoSolicitud] as tblTSolicitud on (tblDG.TipoSolicitud=tblTSolicitud.Id) inner join [dbo].[ClaseTercero] as tblTerc on (tblDG.ClaseTercero=tblTerc.Id) inner join [dbo].[CategoriaTercero] as tblCatTer on (tblDG.CategoriaTercero=tblCatTer.Id) " +
                "inner join [dbo].[TipoDocumentos] as tblTipDocon on (tblDG.TipoIdentificacion=tblTipDocon.Id) inner join [dbo].[Paises] as tblPais  on (tblDG.Pais=tblPais.Id) left join [dbo].[TamañoTercero] as tblTamTerc  on (tblDG.TamanoTercero=tblTamTerc.Id) left join [dbo].[ActividadEconomicaCiiu] as tblActEco  on (tblDG.RazonSocial=tblActEco.Id) " +
                "inner join [dbo].[SINO] as Ofactu on (tblDG.ObligadoFacturarElectronicamente=Ofactu.Id) inner join [dbo].[SINO] as sucOPais on (tblDG.SucursalOtroPais=sucOPais.Id)" +
                "inner join [dbo].[FormularioClienteProveedores] AS fcp ON (fcp.Id=tblDG.IdFormulario) {0}", where);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                DatosGeneralesReporteDto objeto = new DatosGeneralesReporteDto();
                objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                objeto.IdFormulario = Convert.ToInt32(dtInformacion.Rows[0]["IdFormulario"]);
                objeto.FechaDiligenciamiento = "";
                objeto.Empresa = dtInformacion.Rows[0]["Empresa"].ToString();
                objeto.TipoSolicitud = dtInformacion.Rows[0]["TipoSolicitud"].ToString();
                objeto.ClaseTercero = dtInformacion.Rows[0]["ClaseTercero"].ToString();
                objeto.CategoriaTercero = dtInformacion.Rows[0]["CategoriaTercero"].ToString(); objeto.NombreRazonSocial = dtInformacion.Rows[0]["NombreRazonSocial"].ToString();
                objeto.TipoIdentificacion = dtInformacion.Rows[0]["TipoIdentificacion"].ToString();
                objeto.NumeroIdentificacion = dtInformacion.Rows[0]["NumeroIdentificacion"].ToString();
                objeto.DigitoVerificacion = dtInformacion.Rows[0]["DigitoVerificacion"].ToString();
                objeto.Pais = dtInformacion.Rows[0]["Pais"].ToString();
                objeto.Ciudad = dtInformacion.Rows[0]["Ciudad"].ToString();
                objeto.TamanoTercero = dtInformacion.Rows[0]["TamanoTercero"].ToString();
                objeto.ActividadEconimoca = dtInformacion.Rows[0]["RazonSocial"].ToString();
                objeto.DireccionPrincipal = dtInformacion.Rows[0]["DireccionPrincipal"].ToString();
                objeto.CodigoPostal = dtInformacion.Rows[0]["CodigoPostal"].ToString();
                objeto.CorreoElectronico = dtInformacion.Rows[0]["CorreoElectronico"].ToString();
                objeto.Telefono = dtInformacion.Rows[0]["Telefono"].ToString();
                objeto.ObligadoFE = dtInformacion.Rows[0]["ObligadoFacturarElectronicamente"].ToString();
                objeto.CorreoElectronicoFE = dtInformacion.Rows[0]["CorreoElectronicoFacturaEletronica"].ToString();
                objeto.TieneSucursalesOtrosPaises = dtInformacion.Rows[0]["SucursalOtroPais"].ToString();
                objeto.PaisesOtrasSucursales = dtInformacion.Rows[0]["OtroPais"].ToString();
                objeto.PreguntasAdicionales = dtInformacion.Rows[0]["JsonPreguntasPep"];
                objeto.EstadoCivil = dtInformacion.Rows[0]["EstadoCivil"].ToString();
                objeto.ConyugeIdentificacion = dtInformacion.Rows[0]["ConyugeIdentificacion"].ToString();
                var valorDb = dtInformacion.Rows[0]["tipoPago"];
                objeto.tipoPago = valorDb == DBNull.Value ? false : Convert.ToBoolean(valorDb);
                objeto.CertBASC = Convert.ToBoolean(dtInformacion.Rows[0]["CertBASC"]);
                objeto.CertOEA = Convert.ToBoolean(dtInformacion.Rows[0]["CertOEA"]);
                objeto.CertCTPAT = Convert.ToBoolean(dtInformacion.Rows[0]["CertCTPAT"]);
                objeto.CertOtros = Convert.ToBoolean(dtInformacion.Rows[0]["CertOtros"]);
                objeto.CertNinguno = Convert.ToBoolean(dtInformacion.Rows[0]["CertNinguno"]);

                return objeto;
            }

            return null;

        }


        public async Task<RepJunAccDTO> ConsultaInfoRepresentanteslegalesAlertaPaises(int IdFormulario)

        {
            string Consulta = string.Empty;
            object objetojson = null;
            string where = String.Format("Where a.IdFormulario={0}", IdFormulario);




            Consulta = string.Format("SELECT a.[Id] ,a.[IdFormulario] ,a.[JsonRepresentanteLegal]  FROM [dbo].[RepresentanteLegal] as a inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}", where);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                RepJunAccDTO obj = new RepJunAccDTO();
                obj.IdFomrulario = Convert.ToInt32(dtInformacion.Rows[0]["IdFormulario"]);
                obj.Persona = dtInformacion.Rows[0]["JsonRepresentanteLegal"];

                return obj;
            }

            return null;

        }

        public async Task GuardaInfoServiciosAdicionales(int IdFormulario, string NumeroConsulta, object jsonProcuraduria, object jsonRamaJudicial, object ejecucionPenas)
        {
            object jsonProcura = jsonProcuraduria?.ToString() ?? (object)DBNull.Value;
            object jsonRamaJu = jsonRamaJudicial?.ToString() ?? (object)DBNull.Value;
            object jsonEjecuPenas = ejecucionPenas?.ToString() ?? (object)DBNull.Value;

            if (jsonProcura == DBNull.Value && jsonRamaJu == DBNull.Value && jsonEjecuPenas == DBNull.Value)
            {
                return;
            }

            string strConsulta = "UPDATE [dbo].[inspektor_respuesta_ws] " +
                                 "SET [JsonInfoProcuraduria] = @jsonProcura, " +
                                 "[JsonRamaJudicial] = @jsonRamaJu, " +
                                 "[JsonEjecucionPenas] = @jsonEjecuPenas " +
                                 "WHERE [Numero_Consulta] = @NumeroConsulta AND [IdFormulario] = @IdFormulario";

            List<SqlParameter> parametros = new List<SqlParameter>()
    {
        new SqlParameter("@NumeroConsulta", SqlDbType.VarChar) { Value = NumeroConsulta },
        new SqlParameter("@IdFormulario", SqlDbType.Int) { Value = IdFormulario },
        new SqlParameter("@jsonProcura", SqlDbType.NVarChar) { Value = jsonProcura },
        new SqlParameter("@jsonRamaJu", SqlDbType.NVarChar) { Value = jsonRamaJu },
        new SqlParameter("@jsonEjecuPenas", SqlDbType.NVarChar) { Value = jsonEjecuPenas }
    };

            string connString = _configuration.GetConnectionString("SQLConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(strConsulta, connection))
                    {
                        command.Parameters.AddRange(parametros.ToArray());
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al editar los datos generales", ex);
            }
        }

        public async Task<RepJunAccDTO> ConsultaInfoJuntaDirectivalegales(int IdFormulario)

        {

            string Consulta = string.Empty;
            string where = String.Format("Where a.IdFormulario={0}", IdFormulario);

            Consulta = string.Format("SELECT a.[Id] ,a.[IdFormulario] ,a.[JsonJuntaDirectiva]  FROM [dbo].[JuntaDirectiva] as a inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}", where);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                RepJunAccDTO obj = new RepJunAccDTO();
                obj.IdFomrulario = Convert.ToInt32(dtInformacion.Rows[0]["IdFormulario"]);
                obj.Persona = dtInformacion.Rows[0]["JsonJuntaDirectiva"];

                return obj;
            }

            return null;

        }

        public async Task<RepJunAccDTO> ConsultaInfoAccionistasAlertaPaises(int IdFormulario)

        {
            List<RepJunAccDTO> objetolsit = new List<RepJunAccDTO>();
            string Consulta = string.Empty;

            string where = String.Format("Where a.IdFormulario={0}", IdFormulario);
            Consulta = string.Format("SELECT a.[Id] ,a.[IdFormulario] ,a.[JsonAccionistas]  FROM [dbo].[Accionistas] as a inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}", where);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                RepJunAccDTO obj = new RepJunAccDTO();
                obj.IdFomrulario = Convert.ToInt32(dtInformacion.Rows[0]["IdFormulario"]);
                obj.Persona = dtInformacion.Rows[0]["JsonAccionistas"];

                return obj;
            }

            return null;

        }


        public async Task<DatosPagosReporteDto> ConsultaDatosPagoAlertaPaises(int IdFormulario)
        {

            string Consulta = string.Empty;
            string where = String.Format("Where a.IdFormulario={0}", IdFormulario);


            Consulta = string.Format("SELECT a.[Id], a.[IdFormulario], a.[NombreBanco], a.[NumeroCuenta],b.Nombre_es TipoCuenta, a.[CodigoSwift], a.[Ciudad],c.Nombre_es Pais, a.[CorreoElectronico], a.[Sucursal] FROM [dbo].[DatosDePagos] as a inner join [dbo].[TipoCuentaBanco] as b on (b.Id=a.TipoCuenta) inner join [dbo].[Paises] as c on (a.Pais=c.Id) inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}", where);
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                DatosPagosReporteDto objeto = new DatosPagosReporteDto();
                objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                objeto.IdFormulario = Convert.ToInt32(dtInformacion.Rows[0]["IdFormulario"]);
                objeto.NombreBanco = dtInformacion.Rows[0]["NombreBanco"].ToString();
                objeto.NumeroCuenta = dtInformacion.Rows[0]["NumeroCuenta"].ToString();
                objeto.TipoCuenta = dtInformacion.Rows[0]["TipoCuenta"].ToString();
                objeto.CodigoSwift = dtInformacion.Rows[0]["CodigoSwift"].ToString();
                objeto.Ciudad = dtInformacion.Rows[0]["Ciudad"].ToString();
                objeto.Pais = dtInformacion.Rows[0]["Pais"].ToString();
                objeto.CorreoElectronico = dtInformacion.Rows[0]["CorreoElectronico"].ToString();
                objeto.Sucursal = dtInformacion.Rows[0]["Sucursal"].ToString();

                return objeto;
            }

            return null;

        }

        public async Task<object> ConsultaRamaJudicial(ListasAdicionalesDto objRegistro)

        {
            string Consulta = string.Empty;
            object objetojson = null;
            string where = String.Format("Where IdFormulario={0} and [Numero_Consulta]='{1}' and Numero_Identificacion='{2}' ", objRegistro.IdFormulario, objRegistro.NumeroConsulta, objRegistro.NumeroIdentificacion);


            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[Tipo_Tercero] ,[Tipo_Identificacion] ,[Numero_Identificacion],[Nombre],[Numero_Consulta] ,[Coincidencias] ,[Fecha_Consulta],[JsonInfoProcuraduria] ,[JsonRamaJudicial],[JsonEjecucionPenas] FROM [dbo].[inspektor_respuesta_ws] {0}", where);

            DataTable dtInformacion = new DataTable();
            string connString = _configuration.GetConnectionString("SQLConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(Consulta, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dtInformacion));
                    }
                }
            }
            catch (Exception ex)
            {
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception("Error al consultar datos generales de alerta de países: " + ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                objetojson = dtInformacion.Rows[0]["JsonRamaJudicial"];

                return objetojson;
            }

            return objetojson;

        }

        public async Task<object> ConsultaInfoProcuraduria(ListasAdicionalesDto objRegistro)

        {
            string Consulta = string.Empty;
            object objetojson = null;
            string where = String.Format("Where IdFormulario={0} and [Numero_Consulta]='{1}' and Numero_Identificacion='{2}' ", objRegistro.IdFormulario, objRegistro.NumeroConsulta, objRegistro.NumeroIdentificacion);


            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[Tipo_Tercero] ,[Tipo_Identificacion] ,[Numero_Identificacion],[Nombre],[Numero_Consulta] ,[Coincidencias] ,[Fecha_Consulta],[JsonInfoProcuraduria] ,[JsonRamaJudicial],[JsonEjecucionPenas] FROM [dbo].[inspektor_respuesta_ws] {0}", where);

            DataTable dtInformacion = new DataTable();

            string connString = _configuration.GetConnectionString("SQLConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(Consulta, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dtInformacion));
                    }
                }
            }
            catch (Exception ex)
            {
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception("Error al consultar datos generales de alerta de países: " + ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                objetojson = dtInformacion.Rows[0]["JsonInfoProcuraduria"];

                return objetojson;
            }

            return objetojson;

        }

        public async Task<object> ConsultaEjecucionPenas(ListasAdicionalesDto objRegistro)

        {
            string Consulta = string.Empty;
            object objetojson = null;
            string where = String.Format("Where IdFormulario={0} and [Numero_Consulta]='{1}' and Numero_Identificacion='{2}' ", objRegistro.IdFormulario, objRegistro.NumeroConsulta, objRegistro.NumeroIdentificacion);


            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[Tipo_Tercero] ,[Tipo_Identificacion] ,[Numero_Identificacion],[Nombre],[Numero_Consulta] ,[Coincidencias] ,[Fecha_Consulta],[JsonInfoProcuraduria] ,[JsonRamaJudicial],[JsonEjecucionPenas] FROM [dbo].[inspektor_respuesta_ws] {0}", where);

            DataTable dtInformacion = new DataTable();
            string connString = _configuration.GetConnectionString("SQLConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(Consulta, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dtInformacion));
                    }
                }
            }
            catch (Exception ex)
            {
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception("Error al consultar datos generales de alerta de países: " + ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                objetojson = !string.IsNullOrEmpty(dtInformacion.Rows[0]["JsonEjecucionPenas"].ToString()) ? dtInformacion.Rows[0]["JsonEjecucionPenas"] : null;

                return objetojson;
            }

            return objetojson;

        }


        public async Task<DespachoMercanciaReporteDto> ConsulataDespachoMercanciaAlertaPaises(int IdFormulario)
        {
            string Consulta = string.Empty;
            string where = String.Format("Where a.IdFormulario={0}", IdFormulario);


            Consulta = string.Format("SELECT a.[Id] ,a.[IdFormulario] ,a.[DireccionDespacho],b.Nombre_es Pais,a.[Cuidad] ,a.[CodigoPostalEnvio] ,a.[Telefono], a.[EmailCorporativo] FROM [dbo].[DespachoMercancia] as a inner join [dbo].[Paises] as b on(b.Id=a.Pais) inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}", where);
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                DespachoMercanciaReporteDto objeto = new DespachoMercanciaReporteDto();
                objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                objeto.IdFormulario = Convert.ToInt32(dtInformacion.Rows[0]["IdFormulario"]);
                objeto.DireccionDespacho = dtInformacion.Rows[0]["DireccionDespacho"].ToString();
                objeto.Pais = dtInformacion.Rows[0]["Pais"].ToString();
                objeto.Cuidad = dtInformacion.Rows[0]["Cuidad"].ToString();
                objeto.CodigoPostalEnvio = dtInformacion.Rows[0]["CodigoPostalEnvio"].ToString();
                objeto.Telefono = dtInformacion.Rows[0]["Telefono"].ToString();
                objeto.EmailCorporativo = dtInformacion.Rows[0]["EmailCorporativo"].ToString();


                return objeto;
            }
            return null;

        }

        public async Task<bool> GuardaConflictoInteres(ConflictoInteresDto objRegistro)
        {
            bool Respuesta = false;
            try
            {
                if (existeConflictoInteres(objRegistro.Id, objRegistro.IdFormulario))
                {
                    Respuesta = EditaConflictoInteres(objRegistro);
                }
                else
                {
                    Respuesta = GuardaConflictoInt(objRegistro);
                }
            }
            catch (Exception ex)
            {
                Respuesta = false;
                throw new InvalidOperationException("error al CreaR GuardaConflictoInteres");
            }
            return Respuesta;
        }

        private bool GuardaConflictoInt(ConflictoInteresDto objRegistro)
        {
            try
            {
                string query = @"INSERT INTO [dbo].[ConflictoIntereses]
                (
                    [id_formulario],
                    [fecha_declaracion],
                    [ciudad_declaracion],
                    [nombres_apellidos],
                    [cedula],
                    [tipo_vinculacion_samsung],
                    [conoce_procedimiento_conflicto],
                    [razon_no_conocer_procedimiento_conflicto],
                    [entidades_relacion_samsung],
                    [otras_situaciones_conflicto],
                    [actividades_competidor],
                    [razon_actividades_competidor],
                    [relaciones_estado],
                    [razon_relaciones_estado],
                    [regalos_hospitalidad],
                    [razon_regalos_hospitalidad],
                    [parentescos_tercer_grado],
                    [otras_situaciones_afectan_independencia],
                    [razon_otras_situaciones_afectan_independencia],
                    [influencia_indebida_politicas],
                    [influencia_indebida_adjudicaciones],
                    [descuento_reventa],
                    [actividades_regulatorias],
                    [corredor_intermediario],
                    [regalos_funcionarios],
                    [aprueba_transacciones_conflicto]
                )
                VALUES
                (
                    @id_formulario,
                    @fecha_declaracion,
                    @ciudad_declaracion,
                    @nombres_apellidos,
                    @cedula,
                    @tipo_vinculacion_samsung,
                    @conoce_procedimiento_conflicto,
                    @razon_no_conocer_procedimiento_conflicto,
                    @entidades_relacion_samsung,
                    @otras_situaciones_conflicto,
                    @actividades_competidor,
                    @razon_actividades_competidor,
                    @relaciones_estado,
                    @razon_relaciones_estado,
                    @regalos_hospitalidad,
                    @razon_regalos_hospitalidad,
                    @parentescos_tercer_grado,
                    @otras_situaciones_afectan_independencia,
                    @razon_otras_situaciones_afectan_independencia,
                    @influencia_indebida_politicas,
                    @influencia_indebida_adjudicaciones,
                    @descuento_reventa,
                    @actividades_regulatorias,
                    @corredor_intermediario,
                    @regalos_funcionarios,
                    @aprueba_transacciones_conflicto
                );";

                var parametros = new List<SqlParameter>()
{
    new SqlParameter("@id_formulario", SqlDbType.NVarChar, 100) { Value = objRegistro.IdFormulario },
    new SqlParameter("@fecha_declaracion", SqlDbType.Date) { Value = (object)objRegistro.FechaDeclaracion ?? DBNull.Value },
    new SqlParameter("@ciudad_declaracion", SqlDbType.VarChar, 255) { Value = (object)objRegistro.CiudadDeclaracion ?? DBNull.Value },
    new SqlParameter("@nombres_apellidos", SqlDbType.VarChar, 255) { Value = (object)objRegistro.NombresApellidos ?? DBNull.Value },
    new SqlParameter("@cedula", SqlDbType.VarChar, 50) { Value = (object)objRegistro.Cedula ?? DBNull.Value },
    new SqlParameter("@tipo_vinculacion_samsung", SqlDbType.VarChar, 255) { Value = (object)objRegistro.TipoVinculacionSamsung ?? DBNull.Value },
    new SqlParameter("@conoce_procedimiento_conflicto", SqlDbType.Bit) { Value = (object)objRegistro.ConoceProcedimientoConflicto ?? DBNull.Value },
    new SqlParameter("@razon_no_conocer_procedimiento_conflicto", SqlDbType.NVarChar, -1) { Value = (object)objRegistro.RazonNoConocerProcedimientoConflicto ?? DBNull.Value },
    
    // 🔹 Aquí serializamos los arrays como JSON
    new SqlParameter("@entidades_relacion_samsung", SqlDbType.NVarChar, -1) {
        Value = (object)(objRegistro.EntidadesRelacionSamsung != null ? JsonConvert.SerializeObject(objRegistro.EntidadesRelacionSamsung) : DBNull.Value)
    },
    new SqlParameter("@otras_situaciones_conflicto", SqlDbType.VarChar, 500) { Value = (object)objRegistro.OtrasSituacionesConflicto ?? DBNull.Value },
    new SqlParameter("@actividades_competidor", SqlDbType.Bit) { Value = (object)objRegistro.ActividadesCompetidor ?? DBNull.Value },
    new SqlParameter("@razon_actividades_competidor", SqlDbType.NVarChar, -1) { Value = (object)objRegistro.RazonActividadesCompetidor ?? DBNull.Value },
    new SqlParameter("@relaciones_estado", SqlDbType.Bit) { Value = (object)objRegistro.RelacionesEstado ?? DBNull.Value },
    new SqlParameter("@razon_relaciones_estado", SqlDbType.NVarChar, -1) { Value = (object)objRegistro.RazonRelacionesEstado ?? DBNull.Value },
    new SqlParameter("@regalos_hospitalidad", SqlDbType.Bit) { Value = (object)objRegistro.RegalosHospitalidad ?? DBNull.Value },
    new SqlParameter("@razon_regalos_hospitalidad", SqlDbType.NVarChar, -1) { Value = (object)objRegistro.RazonRegalosHospitalidad ?? DBNull.Value },

    // 🔹 Serializamos parentescos también
    new SqlParameter("@parentescos_tercer_grado", SqlDbType.NVarChar, -1) {
        Value = (object)(objRegistro.ParentescosTercerGrado != null ? JsonConvert.SerializeObject(objRegistro.ParentescosTercerGrado) : DBNull.Value)
    },
    new SqlParameter("@otras_situaciones_afectan_independencia", SqlDbType.Bit) { Value = (object)objRegistro.OtrasSituacionesAfectanIndependencia ?? DBNull.Value },
    new SqlParameter("@razon_otras_situaciones_afectan_independencia", SqlDbType.NVarChar, -1) { Value = (object)objRegistro.RazonOtrasSituacionesAfectanIndependencia ?? DBNull.Value },
    new SqlParameter("@influencia_indebida_politicas", SqlDbType.Bit) { Value = (object)objRegistro.InfluenciaIndebidaPoliticas ?? DBNull.Value },
    new SqlParameter("@influencia_indebida_adjudicaciones", SqlDbType.Bit) { Value = (object)objRegistro.InfluenciaIndebidaAdjudicaciones ?? DBNull.Value },
    new SqlParameter("@descuento_reventa", SqlDbType.Bit) { Value = (object)objRegistro.DescuentoReventa ?? DBNull.Value },
    new SqlParameter("@actividades_regulatorias", SqlDbType.Bit) { Value = (object)objRegistro.ActividadesRegulatorias ?? DBNull.Value },
    new SqlParameter("@corredor_intermediario", SqlDbType.Bit) { Value = (object)objRegistro.CorredorIntermediario ?? DBNull.Value },
    new SqlParameter("@regalos_funcionarios", SqlDbType.Bit) { Value = (object)objRegistro.RegalosFuncionarios ?? DBNull.Value },
    new SqlParameter("@aprueba_transacciones_conflicto", SqlDbType.Bit) { Value = (object)objRegistro.ApruebaTransaccionesConflicto ?? DBNull.Value }
};


                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();

                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("Error al insertar registro en ConflictoIntereses", ex);
            }
        }
        private bool EditaConflictoInteres(ConflictoInteresDto objRegistro)
        {
            bool respuesta = false;
            string strConsulta = string.Empty;

            try
            {
                strConsulta = @"UPDATE [dbo].[ConflictoIntereses]
                SET
                    [fecha_declaracion] = @fecha_declaracion,
                    [ciudad_declaracion] = @ciudad_declaracion,
                    [nombres_apellidos] = @nombres_apellidos,
                    [cedula] = @cedula,
                    [tipo_vinculacion_samsung] = @tipo_vinculacion_samsung,
                    [conoce_procedimiento_conflicto] = @conoce_procedimiento_conflicto,
                    [razon_no_conocer_procedimiento_conflicto] = @razon_no_conocer_procedimiento_conflicto,
                    [entidades_relacion_samsung] = @entidades_relacion_samsung,
                    [otras_situaciones_conflicto] = @otras_situaciones_conflicto,
                    [actividades_competidor] = @actividades_competidor,
                    [razon_actividades_competidor] = @razon_actividades_competidor,
                    [relaciones_estado] = @relaciones_estado,
                    [razon_relaciones_estado] = @razon_relaciones_estado,
                    [regalos_hospitalidad] = @regalos_hospitalidad,
                    [razon_regalos_hospitalidad] = @razon_regalos_hospitalidad,
                    [parentescos_tercer_grado] = @parentescos_tercer_grado,
                    [otras_situaciones_afectan_independencia] = @otras_situaciones_afectan_independencia,
                    [razon_otras_situaciones_afectan_independencia] = @razon_otras_situaciones_afectan_independencia,
                    [influencia_indebida_politicas] = @influencia_indebida_politicas,
                    [influencia_indebida_adjudicaciones] = @influencia_indebida_adjudicaciones,
                    [descuento_reventa] = @descuento_reventa,
                    [actividades_regulatorias] = @actividades_regulatorias,
                    [corredor_intermediario] = @corredor_intermediario,
                    [regalos_funcionarios] = @regalos_funcionarios,
                    [aprueba_transacciones_conflicto] = @aprueba_transacciones_conflicto
                WHERE
                    [id] = @id
                    AND [id_formulario] = @id_formulario
            ";

                List<SqlParameter> parametros = new List<SqlParameter>()
        {
            new SqlParameter("@id", SqlDbType.Int) { Value = objRegistro.Id },
            new SqlParameter("@id_formulario", SqlDbType.NVarChar, 100) { Value = objRegistro.IdFormulario },
            new SqlParameter("@fecha_declaracion", SqlDbType.Date) { Value = (object)objRegistro.FechaDeclaracion ?? DBNull.Value },
            new SqlParameter("@ciudad_declaracion", SqlDbType.VarChar, 255) { Value = (object)objRegistro.CiudadDeclaracion ?? DBNull.Value },
            new SqlParameter("@nombres_apellidos", SqlDbType.VarChar, 255) { Value = (object)objRegistro.NombresApellidos ?? DBNull.Value },
            new SqlParameter("@cedula", SqlDbType.VarChar, 50) { Value = (object)objRegistro.Cedula ?? DBNull.Value },
            new SqlParameter("@tipo_vinculacion_samsung", SqlDbType.VarChar, 255) { Value = (object)objRegistro.TipoVinculacionSamsung ?? DBNull.Value},
            new SqlParameter("@conoce_procedimiento_conflicto", SqlDbType.Bit) { Value = (object)objRegistro.ConoceProcedimientoConflicto ?? DBNull.Value },
            new SqlParameter("@razon_no_conocer_procedimiento_conflicto", SqlDbType.NVarChar, -1) { Value = (object)objRegistro.RazonNoConocerProcedimientoConflicto ?? DBNull.Value },
            new SqlParameter("@entidades_relacion_samsung", SqlDbType.NVarChar, -1) { Value = (object)objRegistro.EntidadesRelacionSamsung?.ToString() ?? DBNull.Value },
            new SqlParameter("@otras_situaciones_conflicto", SqlDbType.VarChar, 500) { Value = (object)objRegistro.OtrasSituacionesConflicto ?? DBNull.Value },
            new SqlParameter("@actividades_competidor", SqlDbType.Bit) { Value = (object)objRegistro.ActividadesCompetidor ?? DBNull.Value },
            new SqlParameter("@razon_actividades_competidor", SqlDbType.NVarChar, -1) { Value = (object)objRegistro.RazonActividadesCompetidor ?? DBNull.Value },
            new SqlParameter("@relaciones_estado", SqlDbType.Bit) { Value = (object)objRegistro.RelacionesEstado ?? DBNull.Value },
            new SqlParameter("@razon_relaciones_estado", SqlDbType.NVarChar, -1) { Value = (object)objRegistro.RazonRelacionesEstado ?? DBNull.Value },
            new SqlParameter("@regalos_hospitalidad", SqlDbType.Bit) { Value = (object)objRegistro.RegalosHospitalidad ?? DBNull.Value },
            new SqlParameter("@razon_regalos_hospitalidad", SqlDbType.NVarChar, -1) { Value = (object)objRegistro.RazonRegalosHospitalidad ?? DBNull.Value },
            new SqlParameter("@parentescos_tercer_grado", SqlDbType.NVarChar, -1) { Value = (object)objRegistro.ParentescosTercerGrado?.ToString() ?? DBNull.Value },
            new SqlParameter("@otras_situaciones_afectan_independencia", SqlDbType.Bit) { Value = (object)objRegistro.OtrasSituacionesAfectanIndependencia ?? DBNull.Value },
            new SqlParameter("@razon_otras_situaciones_afectan_independencia", SqlDbType.NVarChar, -1) { Value = (object)objRegistro.RazonOtrasSituacionesAfectanIndependencia ?? DBNull.Value },
            new SqlParameter("@influencia_indebida_politicas", SqlDbType.Bit) { Value = (object)objRegistro.InfluenciaIndebidaPoliticas ?? DBNull.Value },
            new SqlParameter("@influencia_indebida_adjudicaciones", SqlDbType.Bit) { Value = (object)objRegistro.InfluenciaIndebidaAdjudicaciones ?? DBNull.Value },
            new SqlParameter("@descuento_reventa", SqlDbType.Bit) { Value = (object)objRegistro.DescuentoReventa ?? DBNull.Value },
            new SqlParameter("@actividades_regulatorias", SqlDbType.Bit) { Value = (object)objRegistro.ActividadesRegulatorias ?? DBNull.Value },
            new SqlParameter("@corredor_intermediario", SqlDbType.Bit) { Value = (object)objRegistro.CorredorIntermediario ?? DBNull.Value },
            new SqlParameter("@regalos_funcionarios", SqlDbType.Bit) { Value = (object)objRegistro.RegalosFuncionarios ?? DBNull.Value },
            new SqlParameter("@aprueba_transacciones_conflicto", SqlDbType.Bit) { Value = (object)objRegistro.ApruebaTransaccionesConflicto ?? DBNull.Value }
        };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("Error al editar el registro de ConflictoIntereses", ex);
            }

            return respuesta;
        }
        private bool existeConflictoInteres(int Id, int IdFormulario)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[ConflictoIntereses] where id={0} and id_formulario={1}", Id, IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<ConflictoInteresDto> ConsultaConflictoInteres(int IdFormulario)
        {
            string consulta = string.Format(@"
SELECT TOP 1
    [id],
    [id_formulario],
    [fecha_declaracion],
    [ciudad_declaracion],
    [nombres_apellidos],
    [cedula],
    [tipo_vinculacion_samsung],
    [conoce_procedimiento_conflicto],
    [razon_no_conocer_procedimiento_conflicto],
    [entidades_relacion_samsung],
    [otras_situaciones_conflicto],
    [actividades_competidor],
    [razon_actividades_competidor],
    [relaciones_estado],
    [razon_relaciones_estado],
    [regalos_hospitalidad],
    [razon_regalos_hospitalidad],
    [parentescos_tercer_grado],
    [otras_situaciones_afectan_independencia],
    [razon_otras_situaciones_afectan_independencia],
    [influencia_indebida_politicas],
    [influencia_indebida_adjudicaciones],
    [descuento_reventa],
    [actividades_regulatorias],
    [corredor_intermediario],
    [regalos_funcionarios],
    [aprueba_transacciones_conflicto]
FROM [dbo].[ConflictoIntereses]
WHERE id_formulario = '{0}'
ORDER BY id DESC", IdFormulario);


            DataTable dtInformacion = new DataTable();
            ConflictoInteresDto objeto = null;

            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                DataRow row = dtInformacion.Rows[0];
                objeto = new ConflictoInteresDto();
                objeto.Id = Convert.ToInt32(row["id"]);
                objeto.IdFormulario = Convert.ToInt32(row["id_formulario"]);
                objeto.FechaDeclaracion = row["fecha_declaracion"] == DBNull.Value
                ? (DateTime?)null
                : Convert.ToDateTime(row["fecha_declaracion"]);
                objeto.CiudadDeclaracion = row["ciudad_declaracion"] as string;
                objeto.NombresApellidos = row["nombres_apellidos"] as string;
                objeto.Cedula = row["cedula"] as string;
                objeto.TipoVinculacionSamsung = row["tipo_vinculacion_samsung"] as string;
                objeto.ConoceProcedimientoConflicto = row["conoce_procedimiento_conflicto"] == DBNull.Value
                ? (bool?)null
                : Convert.ToBoolean(row["conoce_procedimiento_conflicto"]);
                objeto.RazonNoConocerProcedimientoConflicto = row["razon_no_conocer_procedimiento_conflicto"] as string;
                objeto.EntidadesRelacionSamsung = row["entidades_relacion_samsung"];
                objeto.OtrasSituacionesConflicto = row["otras_situaciones_conflicto"] as string;
                objeto.ActividadesCompetidor = row["actividades_competidor"] == DBNull.Value
                ? (bool?)null
                : Convert.ToBoolean(row["actividades_competidor"]);
                objeto.RazonActividadesCompetidor = row["razon_actividades_competidor"] as string;
                objeto.RelacionesEstado = row["relaciones_estado"] == DBNull.Value
                ? (bool?)null
                : Convert.ToBoolean(row["relaciones_estado"]);
                objeto.RazonRelacionesEstado = row["razon_relaciones_estado"] as string;
                objeto.RegalosHospitalidad = row["regalos_hospitalidad"] == DBNull.Value
                ? (bool?)null
                : Convert.ToBoolean(row["regalos_hospitalidad"]);
                objeto.RazonRegalosHospitalidad = row["razon_regalos_hospitalidad"] as string;
                objeto.ParentescosTercerGrado = row["parentescos_tercer_grado"];
                objeto.OtrasSituacionesAfectanIndependencia = row["otras_situaciones_afectan_independencia"] == DBNull.Value
                ? (bool?)null
                : Convert.ToBoolean(row["otras_situaciones_afectan_independencia"]);
                objeto.RazonOtrasSituacionesAfectanIndependencia = row["razon_otras_situaciones_afectan_independencia"] as string;
                objeto.InfluenciaIndebidaPoliticas = row["influencia_indebida_politicas"] == DBNull.Value
                ? (bool?)null
                : Convert.ToBoolean(row["influencia_indebida_politicas"]);
                objeto.InfluenciaIndebidaAdjudicaciones = row["influencia_indebida_adjudicaciones"] == DBNull.Value
                ? (bool?)null
                : Convert.ToBoolean(row["influencia_indebida_adjudicaciones"]);
                objeto.DescuentoReventa = row["descuento_reventa"] == DBNull.Value
                ? (bool?)null
                : Convert.ToBoolean(row["descuento_reventa"]);
                objeto.ActividadesRegulatorias = row["actividades_regulatorias"] == DBNull.Value
                ? (bool?)null
                : Convert.ToBoolean(row["actividades_regulatorias"]);
                objeto.CorredorIntermediario = row["corredor_intermediario"] == DBNull.Value
                ? (bool?)null
                : Convert.ToBoolean(row["corredor_intermediario"]);
                objeto.RegalosFuncionarios = row["regalos_funcionarios"] == DBNull.Value
                ? (bool?)null
                : Convert.ToBoolean(row["regalos_funcionarios"]);
                objeto.ApruebaTransaccionesConflicto = row["aprueba_transacciones_conflicto"] == DBNull.Value
                ? (bool?)null
                : Convert.ToBoolean(row["aprueba_transacciones_conflicto"]);
            }

            return objeto;
        }
        private bool GuardaInformacionComple(InformacionComplementariaDto objRegistro)
        {
            try
            {
                string query = @"
                INSERT INTO [dbo].[InformacionComplementaria]
                (
                    [IdFormulario],
                    [ActivosVirtuales],
                    [GrandesCantidadesEfectivo],
                    [InvestigadoViolacionLeyesAnticorrupcion],
                    [DeclaracionNoToleranciaCorrupcion],
                    [ExtensionColaboradoresPolitica],
                    [PoliticaAportesDonaciones],
                    [ContratadoTercerosOrganizacion],
                    [ObligadaSistemaPrevencionLAFT],
                    [TieneSistemaPrevencionLAFT],
                    [CasoRespuestaSistemaLAFT],
                    [AdopcionPoliticasLAFT],
                    [NombramientoOficialCumplimiento],
                    [MedidasDebidaDiligencia],
                    [IdentificacionEvaluacionRiesgos],
                    [IdentificacionReporteSospechosas],
                    [PoliticasCapacitacionLAFT],
                    [ObligadoAutocontrolLAFT],
                    [ObligadoProgramaPTEE],
                    [AdopcionPoliticasOrganoDireccion],
                    [EstablecimientoMedidasDebidaDiligencia],
                    [IdentificacionReportesOperSospechosas],
                    [RiesgosCorrupcionSobornoTransnacional],
                    [RiesgosLAFT],
                    [PoliticasCapacitacion]
                )
                VALUES
                (
                    @IdFormulario,
                    @ActivosVirtuales,
                    @GrandesCantidadesEfectivo,
                    @InvestigadoViolacionLeyesAnticorrupcion,
                    @DeclaracionNoToleranciaCorrupcion,
                    @ExtensionColaboradoresPolitica,
                    @PoliticaAportesDonaciones,
                    @ContratadoTercerosOrganizacion,
                    @ObligadaSistemaPrevencionLAFT,
                    @TieneSistemaPrevencionLAFT,
                    @CasoRespuestaSistemaLAFT,
                    @AdopcionPoliticasLAFT,
                    @NombramientoOficialCumplimiento,
                    @MedidasDebidaDiligencia,
                    @IdentificacionEvaluacionRiesgos,
                    @IdentificacionReporteSospechosas,
                    @PoliticasCapacitacionLAFT,
                    @ObligadoAutocontrolLAFT,
                    @ObligadoProgramaPTEE,
                    @AdopcionPoliticasOrganoDireccion,
                    @EstablecimientoMedidasDebidaDiligencia,
                    @IdentificacionReportesOperSospechosas,
                    @RiesgosCorrupcionSobornoTransnacional,
                    @RiesgosLAFT,
                    @PoliticasCapacitacion
                );";

                var parametros = new List<SqlParameter>()
                {
                    new SqlParameter("@IdFormulario", SqlDbType.Int) { Value = objRegistro.IdFormulario },
                    new SqlParameter("@ActivosVirtuales", SqlDbType.Bit) { Value = objRegistro.ActivosVirtuales },
                    new SqlParameter("@GrandesCantidadesEfectivo", SqlDbType.Bit) { Value = objRegistro.GrandesCantidadesEfectivo },
                    new SqlParameter("@InvestigadoViolacionLeyesAnticorrupcion", SqlDbType.Bit) { Value = objRegistro.InvestigadoViolacionLeyesAnticorrupcion },
                    new SqlParameter("@DeclaracionNoToleranciaCorrupcion", SqlDbType.Bit) { Value = objRegistro.DeclaracionNoToleranciaCorrupcion },
                    new SqlParameter("@ExtensionColaboradoresPolitica", SqlDbType.Bit) { Value = objRegistro.ExtensionColaboradoresPolitica },
                    new SqlParameter("@PoliticaAportesDonaciones", SqlDbType.Bit) { Value = objRegistro.PoliticaAportesDonaciones },
                    new SqlParameter("@ContratadoTercerosOrganizacion", SqlDbType.Bit) { Value = objRegistro.ContratadoTercerosOrganizacion },
                    new SqlParameter("@ObligadaSistemaPrevencionLAFT", SqlDbType.Bit) { Value = objRegistro.ObligadaSistemaPrevencionLAFT },
                    new SqlParameter("@TieneSistemaPrevencionLAFT", SqlDbType.Bit) { Value = objRegistro.TieneSistemaPrevencionLAFT },
                    new SqlParameter("@CasoRespuestaSistemaLAFT", SqlDbType.Bit) { Value = objRegistro.CasoRespuestaSistemaLAFT },
                    new SqlParameter("@AdopcionPoliticasLAFT", SqlDbType.Bit) { Value = objRegistro.AdopcionPoliticasLAFT },
                    new SqlParameter("@NombramientoOficialCumplimiento", SqlDbType.Bit) { Value = objRegistro.NombramientoOficialCumplimiento },
                    new SqlParameter("@MedidasDebidaDiligencia", SqlDbType.Bit) { Value = objRegistro.MedidasDebidaDiligencia },
                    new SqlParameter("@IdentificacionEvaluacionRiesgos", SqlDbType.Bit) { Value = objRegistro.IdentificacionEvaluacionRiesgos },
                    new SqlParameter("@IdentificacionReporteSospechosas", SqlDbType.Bit) { Value = objRegistro.IdentificacionReporteSospechosas },
                    new SqlParameter("@PoliticasCapacitacionLAFT", SqlDbType.Bit) { Value = objRegistro.PoliticasCapacitacionLAFT },
                    new SqlParameter("@ObligadoAutocontrolLAFT", SqlDbType.Bit) { Value = objRegistro.ObligadoAutocontrolLAFT },
                    new SqlParameter("@ObligadoProgramaPTEE", SqlDbType.Bit) { Value = objRegistro.ObligadoProgramaPTEE },
                    new SqlParameter("@AdopcionPoliticasOrganoDireccion", SqlDbType.Bit) { Value = objRegistro.AdopcionPoliticasOrganoDireccion },
                    new SqlParameter("@EstablecimientoMedidasDebidaDiligencia", SqlDbType.Bit) { Value = objRegistro.EstablecimientoMedidasDebidaDiligencia },
                    new SqlParameter("@IdentificacionReportesOperSospechosas", SqlDbType.Bit) { Value = objRegistro.IdentificacionReportesOperSospechosas },
                    new SqlParameter("@RiesgosCorrupcionSobornoTransnacional", SqlDbType.Bit) { Value = objRegistro.RiesgosCorrupcionSobornoTransnacional },
                    new SqlParameter("@RiesgosLAFT", SqlDbType.Bit) { Value = objRegistro.RiesgosLAFT },
                    new SqlParameter("@PoliticasCapacitacion", SqlDbType.Bit) { Value = objRegistro.PoliticasCapacitacion }
                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();

                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("Error al insertar registro en InformacionComplementaria", ex);
            }
        }

        private bool EditaInformacionComplementaria(InformacionComplementariaDto objRegistro)
        {
            try
            {
                string query = @"
                UPDATE [dbo].[InformacionComplementaria]
                SET
                    [IdFormulario] = @IdFormulario,
                    [ActivosVirtuales] = @ActivosVirtuales,
                    [GrandesCantidadesEfectivo] = @GrandesCantidadesEfectivo,
                    [InvestigadoViolacionLeyesAnticorrupcion] = @InvestigadoViolacionLeyesAnticorrupcion,
                    [DeclaracionNoToleranciaCorrupcion] = @DeclaracionNoToleranciaCorrupcion,
                    [ExtensionColaboradoresPolitica] = @ExtensionColaboradoresPolitica,
                    [PoliticaAportesDonaciones] = @PoliticaAportesDonaciones,
                    [ContratadoTercerosOrganizacion] = @ContratadoTercerosOrganizacion,
                    [ObligadaSistemaPrevencionLAFT] = @ObligadaSistemaPrevencionLAFT,
                    [TieneSistemaPrevencionLAFT] = @TieneSistemaPrevencionLAFT,
                    [CasoRespuestaSistemaLAFT] = @CasoRespuestaSistemaLAFT,
                    [AdopcionPoliticasLAFT] = @AdopcionPoliticasLAFT,
                    [NombramientoOficialCumplimiento] = @NombramientoOficialCumplimiento,
                    [MedidasDebidaDiligencia] = @MedidasDebidaDiligencia,
                    [IdentificacionEvaluacionRiesgos] = @IdentificacionEvaluacionRiesgos,
                    [IdentificacionReporteSospechosas] = @IdentificacionReporteSospechosas,
                    [PoliticasCapacitacionLAFT] = @PoliticasCapacitacionLAFT,
                    [ObligadoAutocontrolLAFT] = @ObligadoAutocontrolLAFT,
                    [ObligadoProgramaPTEE] = @ObligadoProgramaPTEE,
                    [AdopcionPoliticasOrganoDireccion] = @AdopcionPoliticasOrganoDireccion,
                    [EstablecimientoMedidasDebidaDiligencia] = @EstablecimientoMedidasDebidaDiligencia,
                    [IdentificacionReportesOperSospechosas] = @IdentificacionReportesOperSospechosas,
                    [RiesgosCorrupcionSobornoTransnacional] = @RiesgosCorrupcionSobornoTransnacional,
                    [RiesgosLAFT] = @RiesgosLAFT,
                    [PoliticasCapacitacion] = @PoliticasCapacitacion
                WHERE
                    [Id] = @Id
                    AND [IdFormulario] = @IdFormulario
            ";

                var parametros = new List<SqlParameter>()
                {
                    new SqlParameter("@Id", SqlDbType.Int) { Value = objRegistro.Id },
                    new SqlParameter("@IdFormulario", SqlDbType.Int) { Value = objRegistro.IdFormulario },
                    new SqlParameter("@ActivosVirtuales", SqlDbType.Bit) { Value = objRegistro.ActivosVirtuales },
                    new SqlParameter("@GrandesCantidadesEfectivo", SqlDbType.Bit) { Value = objRegistro.GrandesCantidadesEfectivo },
                    new SqlParameter("@InvestigadoViolacionLeyesAnticorrupcion", SqlDbType.Bit) { Value = objRegistro.InvestigadoViolacionLeyesAnticorrupcion },
                    new SqlParameter("@DeclaracionNoToleranciaCorrupcion", SqlDbType.Bit) { Value = objRegistro.DeclaracionNoToleranciaCorrupcion },
                    new SqlParameter("@ExtensionColaboradoresPolitica", SqlDbType.Bit) { Value = objRegistro.ExtensionColaboradoresPolitica },
                    new SqlParameter("@PoliticaAportesDonaciones", SqlDbType.Bit) { Value = objRegistro.PoliticaAportesDonaciones },
                    new SqlParameter("@ContratadoTercerosOrganizacion", SqlDbType.Bit) { Value = objRegistro.ContratadoTercerosOrganizacion },
                    new SqlParameter("@ObligadaSistemaPrevencionLAFT", SqlDbType.Bit) { Value = objRegistro.ObligadaSistemaPrevencionLAFT },
                    new SqlParameter("@TieneSistemaPrevencionLAFT", SqlDbType.Bit) { Value = objRegistro.TieneSistemaPrevencionLAFT },
                    new SqlParameter("@CasoRespuestaSistemaLAFT", SqlDbType.Bit ) {Value = objRegistro.CasoRespuestaSistemaLAFT },
                    new SqlParameter("@AdopcionPoliticasLAFT", SqlDbType.Bit) { Value = objRegistro.AdopcionPoliticasLAFT },
                    new SqlParameter("@NombramientoOficialCumplimiento", SqlDbType.Bit) { Value = objRegistro.NombramientoOficialCumplimiento },
                    new SqlParameter("@MedidasDebidaDiligencia", SqlDbType.Bit) { Value = objRegistro.MedidasDebidaDiligencia },
                    new SqlParameter("@IdentificacionEvaluacionRiesgos", SqlDbType.Bit) { Value = objRegistro.IdentificacionEvaluacionRiesgos },
                    new SqlParameter("@IdentificacionReporteSospechosas", SqlDbType.Bit) { Value = objRegistro.IdentificacionReporteSospechosas },
                    new SqlParameter("@PoliticasCapacitacionLAFT", SqlDbType.Bit) { Value = objRegistro.PoliticasCapacitacionLAFT },
                    new SqlParameter("@ObligadoAutocontrolLAFT", SqlDbType.Bit) { Value = objRegistro.ObligadoAutocontrolLAFT },
                    new SqlParameter("@ObligadoProgramaPTEE", SqlDbType.Bit) { Value = objRegistro.ObligadoProgramaPTEE },
                    new SqlParameter("@AdopcionPoliticasOrganoDireccion", SqlDbType.Bit) { Value = objRegistro.AdopcionPoliticasOrganoDireccion },
                    new SqlParameter("@EstablecimientoMedidasDebidaDiligencia", SqlDbType.Bit) { Value = objRegistro.EstablecimientoMedidasDebidaDiligencia },
                    new SqlParameter("@IdentificacionReportesOperSospechosas", SqlDbType.Bit) { Value = objRegistro.IdentificacionReportesOperSospechosas },
                    new SqlParameter("@RiesgosCorrupcionSobornoTransnacional", SqlDbType.Bit) { Value = objRegistro.RiesgosCorrupcionSobornoTransnacional },
                    new SqlParameter("@RiesgosLAFT", SqlDbType.Bit) { Value = objRegistro.RiesgosLAFT },
                    new SqlParameter("@PoliticasCapacitacion", SqlDbType.Bit) { Value = objRegistro.PoliticasCapacitacion }
                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();

                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("Error al editar el registro en InformacionComplementaria", ex);
            }
        }

        private bool existeInformacionComplementaria(int Id, int IdFormulario)
        {
            string consulta = string.Format(
                "SELECT * FROM [dbo].[InformacionComplementaria] WHERE [Id] = {0} AND [IdFormulario] = {1}",
                Id, IdFormulario
            );

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            return (dtInformacion.Rows.Count > 0);
        }

        public async Task<InformacionComplementariaDto> ConsultaInformacionComplementaria(int IdFormulario)
        {
            string consulta = string.Format(@"
            SELECT
                [Id],
                [IdFormulario],
                [ActivosVirtuales],
                [GrandesCantidadesEfectivo],

                [InvestigadoViolacionLeyesAnticorrupcion],
                [DeclaracionNoToleranciaCorrupcion],
                [ExtensionColaboradoresPolitica],
                [PoliticaAportesDonaciones],
                [ContratadoTercerosOrganizacion],

                [ObligadaSistemaPrevencionLAFT],
                [TieneSistemaPrevencionLAFT],
                [CasoRespuestaSistemaLAFT],
                [AdopcionPoliticasLAFT],
                [NombramientoOficialCumplimiento],
                [MedidasDebidaDiligencia],
                [IdentificacionEvaluacionRiesgos],
                [IdentificacionReporteSospechosas],
                [PoliticasCapacitacionLAFT],

                [ObligadoAutocontrolLAFT],
                [ObligadoProgramaPTEE],
                [AdopcionPoliticasOrganoDireccion],
                [EstablecimientoMedidasDebidaDiligencia],
                [IdentificacionReportesOperSospechosas],
                [RiesgosCorrupcionSobornoTransnacional],
                [RiesgosLAFT],
                [PoliticasCapacitacion]
            FROM [dbo].[InformacionComplementaria]
            WHERE [IdFormulario] = {0}", IdFormulario);

            DataTable dtInformacion = new DataTable();
            InformacionComplementariaDto objeto = null;

            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                DataRow row = dtInformacion.Rows[0];
                objeto = new InformacionComplementariaDto();
                objeto.Id = Convert.ToInt32(row["Id"]);
                objeto.IdFormulario = Convert.ToInt32(row["IdFormulario"]);
                var valorDbAc = dtInformacion.Rows[0]["ActivosVirtuales"];
                objeto.ActivosVirtuales = valorDbAc == DBNull.Value ? false : Convert.ToBoolean(valorDbAc);
                var valorDb = dtInformacion.Rows[0]["GrandesCantidadesEfectivo"];
                objeto.GrandesCantidadesEfectivo = valorDb == DBNull.Value ? false : Convert.ToBoolean(valorDb);
                objeto.InvestigadoViolacionLeyesAnticorrupcion = Convert.ToBoolean(row["InvestigadoViolacionLeyesAnticorrupcion"]);
                objeto.DeclaracionNoToleranciaCorrupcion = Convert.ToBoolean(row["DeclaracionNoToleranciaCorrupcion"]);
                objeto.ExtensionColaboradoresPolitica = Convert.ToBoolean(row["ExtensionColaboradoresPolitica"]);
                objeto.PoliticaAportesDonaciones = Convert.ToBoolean(row["PoliticaAportesDonaciones"]);
                objeto.ContratadoTercerosOrganizacion = Convert.ToBoolean(row["ContratadoTercerosOrganizacion"]);
                objeto.ObligadaSistemaPrevencionLAFT = Convert.ToBoolean(row["ObligadaSistemaPrevencionLAFT"]);
                objeto.TieneSistemaPrevencionLAFT = Convert.ToBoolean(row["TieneSistemaPrevencionLAFT"]);
                objeto.CasoRespuestaSistemaLAFT = Convert.ToBoolean(row["CasoRespuestaSistemaLAFT"]);
                objeto.AdopcionPoliticasLAFT = Convert.ToBoolean(row["AdopcionPoliticasLAFT"]);
                objeto.NombramientoOficialCumplimiento = Convert.ToBoolean(row["NombramientoOficialCumplimiento"]);
                objeto.MedidasDebidaDiligencia = Convert.ToBoolean(row["MedidasDebidaDiligencia"]);
                objeto.IdentificacionEvaluacionRiesgos = Convert.ToBoolean(row["IdentificacionEvaluacionRiesgos"]);
                objeto.IdentificacionReporteSospechosas = Convert.ToBoolean(row["IdentificacionReporteSospechosas"]);
                objeto.PoliticasCapacitacionLAFT = Convert.ToBoolean(row["PoliticasCapacitacionLAFT"]);
                objeto.ObligadoAutocontrolLAFT = Convert.ToBoolean(row["ObligadoAutocontrolLAFT"]);
                objeto.ObligadoProgramaPTEE = Convert.ToBoolean(row["ObligadoProgramaPTEE"]);
                objeto.AdopcionPoliticasOrganoDireccion = Convert.ToBoolean(row["AdopcionPoliticasOrganoDireccion"]);
                objeto.EstablecimientoMedidasDebidaDiligencia = Convert.ToBoolean(row["EstablecimientoMedidasDebidaDiligencia"]);
                objeto.IdentificacionReportesOperSospechosas = Convert.ToBoolean(row["IdentificacionReportesOperSospechosas"]);
                objeto.RiesgosCorrupcionSobornoTransnacional = Convert.ToBoolean(row["RiesgosCorrupcionSobornoTransnacional"]);
                objeto.RiesgosLAFT = Convert.ToBoolean(row["RiesgosLAFT"]);
                objeto.PoliticasCapacitacion = Convert.ToBoolean(row["PoliticasCapacitacion"]);
            }

            return objeto;
        }

        public async Task<bool> GuardaInformacionComplementaria(InformacionComplementariaDto objRegistro)
        {
            bool Respuesta = false;
            try
            {
                if (existeInformacionComplementaria(objRegistro.Id, objRegistro.IdFormulario))
                {
                    Respuesta = EditaInformacionComplementaria(objRegistro);
                }
                else
                {
                    Respuesta = GuardaInformacionComple(objRegistro);
                }
            }
            catch (Exception ex)
            {
                Respuesta = false;
                throw new InvalidOperationException("Error al crear/editar InformacionComplementaria", ex);
            }
            return Respuesta;
        }

        private bool InsertarInformacionFinanciera(InformacionFinancieraDto objRegistro)
        {
            try
            {
                string query = @"
                INSERT INTO [dbo].[InformacionFinanciera]
                (
                    [IdFormulario],
                    [Patrimonio],
                    [Activos],
                    [IngresosMensuales],
                    [EgresosMensuales],
                    [ActivosVirtuales],
                    [GrandesCantidadesEfectivo]
                )
                VALUES
                (
                    @IdFormulario,
                    @Patrimonio,
                    @Activos,
                    @IngresosMensuales,
                    @EgresosMensuales,
                    @ActivosVirtuales,
                    @GrandesCantidadesEfectivo
                );";

                var parametros = new List<SqlParameter>()
                {
                    new SqlParameter("@IdFormulario", SqlDbType.Int) { Value = objRegistro.IdFormulario },
                    new SqlParameter("@Patrimonio", SqlDbType.Decimal) { Value = objRegistro.Patrimonio },
                    new SqlParameter("@Activos", SqlDbType.Decimal) { Value = objRegistro.Activos },
                    new SqlParameter("@IngresosMensuales", SqlDbType.Decimal) { Value = objRegistro.IngresosMensuales },
                    new SqlParameter("@EgresosMensuales", SqlDbType.Decimal) { Value = objRegistro.EgresosMensuales },
                    new SqlParameter("@ActivosVirtuales", SqlDbType.Bit) { Value = (object)objRegistro.ActivosVirtuales ?? DBNull.Value },
                    new SqlParameter("@GrandesCantidadesEfectivo", SqlDbType.Bit) { Value = (object)objRegistro.GrandesCantidadesEfectivo ?? DBNull.Value },
                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();

                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("Error al insertar InformacionFinanciera", ex);
            }
        }

        private bool EditarInformacionFinanciera(InformacionFinancieraDto objRegistro)
        {
            try
            {
                string query = @"
                UPDATE [dbo].[InformacionFinanciera]
                SET
                    [IdFormulario] = @IdFormulario,
                    [Patrimonio] = @Patrimonio,
                    [Activos] = @Activos,
                    [IngresosMensuales] = @IngresosMensuales,
                    [EgresosMensuales] = @EgresosMensuales,
                    [ActivosVirtuales] = @ActivosVirtuales,
                    [GrandesCantidadesEfectivo] = @GrandesCantidadesEfectivo
                WHERE
                    [Id] = @Id
                    AND [IdFormulario] = @IdFormulario;
            ";

                var parametros = new List<SqlParameter>()
                {
                    new SqlParameter("@Id", SqlDbType.Int) { Value = objRegistro.Id },
                    new SqlParameter("@IdFormulario", SqlDbType.Int) { Value = objRegistro.IdFormulario },
                    new SqlParameter("@Patrimonio", SqlDbType.Decimal) { Value = objRegistro.Patrimonio },
                    new SqlParameter("@Activos", SqlDbType.Decimal) { Value = objRegistro.Activos },
                    new SqlParameter("@IngresosMensuales", SqlDbType.Decimal) { Value = objRegistro.IngresosMensuales },
                    new SqlParameter("@EgresosMensuales", SqlDbType.Decimal) { Value = objRegistro.EgresosMensuales },
                    new SqlParameter("@ActivosVirtuales", SqlDbType.Bit) { Value = objRegistro.ActivosVirtuales },
                    new SqlParameter("@GrandesCantidadesEfectivo", SqlDbType.Bit) { Value = objRegistro.GrandesCantidadesEfectivo },
                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();

                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("Error al editar InformacionFinanciera", ex);
            }
        }

        private bool ExisteInformacionFinanciera(int Id, int IdFormulario)
        {
            string consulta = string.Format(
                "SELECT * FROM [dbo].[InformacionFinanciera] WHERE [Id] = {0} AND [IdFormulario] = {1}",
                Id, IdFormulario
            );

            DataTable dt = new DataTable();
            try
            {
                cDataBase.conectar();
                dt = cDataBase.ejecutarConsulta(consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dt.Rows.Clear();
                dt.Columns.Clear();
                throw new Exception(ex.Message);
            }

            return (dt.Rows.Count > 0);
        }

        public async Task<InformacionFinancieraDto> ConsultaInformacionFinanciera(int IdFormulario)
        {
            string consulta = string.Format(@"
                    SELECT
                        [Id],
                        [IdFormulario],
                        [Patrimonio],
                        [Activos],
                        [IngresosMensuales],
                        [EgresosMensuales],
                        [ActivosVirtuales],
                        [GrandesCantidadesEfectivo]
                    FROM [dbo].[InformacionFinanciera]
                    WHERE [IdFormulario] = {0}", IdFormulario);

            DataTable dt = new DataTable();
            InformacionFinancieraDto objeto = null;

            try
            {
                cDataBase.conectar();
                dt = cDataBase.ejecutarConsulta(consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dt.Rows.Clear();
                dt.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                objeto = new InformacionFinancieraDto
                {
                    Id = Convert.ToInt32(row["Id"]),
                    IdFormulario = Convert.ToInt32(row["IdFormulario"]),
                    Patrimonio = Convert.ToDecimal(row["Patrimonio"]),
                    Activos = Convert.ToDecimal(row["Activos"]),
                    IngresosMensuales = Convert.ToDecimal(row["IngresosMensuales"]),
                    EgresosMensuales = Convert.ToDecimal(row["EgresosMensuales"]),
                    ActivosVirtuales = row["ActivosVirtuales"] == DBNull.Value ? false : Convert.ToBoolean(row["ActivosVirtuales"]),
                    GrandesCantidadesEfectivo = row["GrandesCantidadesEfectivo"] == DBNull.Value ? false : Convert.ToBoolean(row["GrandesCantidadesEfectivo"])

                };
            }

            return objeto;
        }

        public async Task<bool> GuardaInformacionFinanciera(InformacionFinancieraDto objRegistro)
        {
            bool respuesta = false;
            try
            {
                if (ExisteInformacionFinanciera(objRegistro.Id, objRegistro.IdFormulario))
                {
                    respuesta = EditarInformacionFinanciera(objRegistro);
                }
                else
                {
                    respuesta = InsertarInformacionFinanciera(objRegistro);
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                throw new InvalidOperationException("Error al guardar/actualizar InformacionFinanciera", ex);
            }
            return respuesta;
        }

        private bool InsertarDatosRevisorFiscal(DatosRevisorFiscalDto objRegistro)
        {
            try
            {
                string query = @"
                INSERT INTO [dbo].[DatosRevisorFiscal]
                (
                    [IdFormulario],
                    [TieneRevisorFiscal],
                    [JustificarRespuesta],
                    [RevisorFiscalAdscritoFirma],
                    [NombreFirma],
                    [NombreCompletoApellidos],
                    [TipoID],
                    [NumeroID],
                    [Telefono],
                    [Ciudad],
                    [Direccion],
                    [Email]
                )
                VALUES
                (
                    @IdFormulario,
                    @TieneRevisorFiscal,
                    @JustificarRespuesta,
                    @RevisorFiscalAdscritoFirma,
                    @NombreFirma,
                    @NombreCompletoApellidos,
                    @TipoID,
                    @NumeroID,
                    @Telefono,
                    @Ciudad,
                    @Direccion,
                    @Email
                );";

                var parametros = new List<SqlParameter>()
                {
                    new SqlParameter("@IdFormulario", SqlDbType.Int) { Value = objRegistro.IdFormulario },
                    new SqlParameter("@TieneRevisorFiscal", SqlDbType.Bit) { Value = objRegistro.TieneRevisorFiscal },
                    new SqlParameter("@JustificarRespuesta", SqlDbType.NVarChar, 500){ Value = (object)objRegistro.JustificarRespuesta ?? DBNull.Value },
                    new SqlParameter("@RevisorFiscalAdscritoFirma", SqlDbType.Bit){ Value = objRegistro.RevisorFiscalAdscritoFirma },
                    new SqlParameter("@NombreFirma", SqlDbType.NVarChar, 255){ Value = (object)objRegistro.NombreFirma ?? DBNull.Value },
                    new SqlParameter("@NombreCompletoApellidos", SqlDbType.NVarChar, 255){ Value = (object)objRegistro.NombreCompletoApellidos ?? DBNull.Value },
                    new SqlParameter("@TipoID", SqlDbType.NVarChar, 50){ Value = (object)objRegistro.TipoID ?? DBNull.Value },
                    new SqlParameter("@NumeroID", SqlDbType.NVarChar, 50){ Value = (object)objRegistro.NumeroID ?? DBNull.Value },
                    new SqlParameter("@Telefono", SqlDbType.NVarChar, 50){ Value = (object)objRegistro.Telefono ?? DBNull.Value },
                    new SqlParameter("@Ciudad", SqlDbType.NVarChar, 100){ Value = (object)objRegistro.Ciudad ?? DBNull.Value },
                    new SqlParameter("@Direccion", SqlDbType.NVarChar, 255){ Value = (object)objRegistro.Direccion ?? DBNull.Value },
                    new SqlParameter("@Email", SqlDbType.NVarChar, 255){ Value = (object)objRegistro.Email ?? DBNull.Value }
                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();

                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("Error al insertar DatosRevisorFiscal", ex);
            }
        }

        private bool EditarDatosRevisorFiscal(DatosRevisorFiscalDto objRegistro)
        {
            try
            {
                string query = @"
                    UPDATE [dbo].[DatosRevisorFiscal]
                    SET
                        [IdFormulario] = @IdFormulario,
                        [TieneRevisorFiscal] = @TieneRevisorFiscal,
                        [JustificarRespuesta] = @JustificarRespuesta,
                        [RevisorFiscalAdscritoFirma] = @RevisorFiscalAdscritoFirma,
                        [NombreFirma] = @NombreFirma,
                        [NombreCompletoApellidos] = @NombreCompletoApellidos,
                        [TipoID] = @TipoID,
                        [NumeroID] = @NumeroID,
                        [Telefono] = @Telefono,
                        [Ciudad] = @Ciudad,
                        [Direccion] = @Direccion,
                        [Email] = @Email
                    WHERE
                        [Id] = @Id
                        AND [IdFormulario] = @IdFormulario;
                ";

                var parametros = new List<SqlParameter>()
                {
                    new SqlParameter("@Id", SqlDbType.Int) { Value = objRegistro.Id },
                    new SqlParameter("@IdFormulario", SqlDbType.Int) { Value = objRegistro.IdFormulario },
                    new SqlParameter("@TieneRevisorFiscal", SqlDbType.Bit) { Value = objRegistro.TieneRevisorFiscal },
                    new SqlParameter("@JustificarRespuesta", SqlDbType.NVarChar, 500){ Value = (object)objRegistro.JustificarRespuesta ?? DBNull.Value },
                    new SqlParameter("@RevisorFiscalAdscritoFirma", SqlDbType.Bit){ Value = objRegistro.RevisorFiscalAdscritoFirma },
                    new SqlParameter("@NombreFirma", SqlDbType.NVarChar, 255){ Value = (object)objRegistro.NombreFirma ?? DBNull.Value },
                    new SqlParameter("@NombreCompletoApellidos", SqlDbType.NVarChar, 255){ Value = (object)objRegistro.NombreCompletoApellidos ?? DBNull.Value },
                    new SqlParameter("@TipoID", SqlDbType.NVarChar, 50){ Value = (object)objRegistro.TipoID ?? DBNull.Value },
                    new SqlParameter("@NumeroID", SqlDbType.NVarChar, 50){ Value = (object)objRegistro.NumeroID ?? DBNull.Value },
                    new SqlParameter("@Telefono", SqlDbType.NVarChar, 50){ Value = (object)objRegistro.Telefono ?? DBNull.Value },
                    new SqlParameter("@Ciudad", SqlDbType.NVarChar, 100){ Value = (object)objRegistro.Ciudad ?? DBNull.Value },
                    new SqlParameter("@Direccion", SqlDbType.NVarChar, 255){ Value = (object)objRegistro.Direccion ?? DBNull.Value },
                    new SqlParameter("@Email", SqlDbType.NVarChar, 255){ Value = (object)objRegistro.Email ?? DBNull.Value }
                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();

                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("Error al editar DatosRevisorFiscal", ex);
            }
        }

        private bool ExisteDatosRevisorFiscal(int Id, int IdFormulario)
        {
            string consulta = string.Format(@" SELECT * FROM [dbo].[DatosRevisorFiscal] WHERE [Id] = {0} AND [IdFormulario] = {1}", Id, IdFormulario);

            DataTable dt = new DataTable();
            try
            {
                cDataBase.conectar();
                dt = cDataBase.ejecutarConsulta(consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dt.Rows.Clear();
                dt.Columns.Clear();
                throw new Exception(ex.Message);
            }

            return (dt.Rows.Count > 0);
        }

        public async Task<DatosRevisorFiscalDto> ConsultaDatosRevisorFiscal(int IdFormulario)
        {
            string consulta = string.Format(@"
                                        SELECT
                                            [Id],
                                            [IdFormulario],
                                            [TieneRevisorFiscal],
                                            [JustificarRespuesta],
                                            [RevisorFiscalAdscritoFirma],
                                            [NombreFirma],
                                            [NombreCompletoApellidos],
                                            [TipoID],
                                            [NumeroID],
                                            [Telefono],
                                            [Ciudad],
                                            [Direccion],
                                            [Email]
                                        FROM [dbo].[DatosRevisorFiscal]
                                        WHERE [IdFormulario] = {0}", IdFormulario);

            DataTable dt = new DataTable();
            DatosRevisorFiscalDto objeto = null;

            try
            {
                cDataBase.conectar();
                dt = cDataBase.ejecutarConsulta(consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dt.Rows.Clear();
                dt.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                objeto = new DatosRevisorFiscalDto
                {
                    Id = Convert.ToInt32(row["Id"]),
                    IdFormulario = Convert.ToInt32(row["IdFormulario"]),
                    TieneRevisorFiscal = Convert.ToBoolean(row["TieneRevisorFiscal"]),
                    JustificarRespuesta = row["JustificarRespuesta"] as string,
                    RevisorFiscalAdscritoFirma = Convert.ToBoolean(row["RevisorFiscalAdscritoFirma"]),
                    NombreFirma = row["NombreFirma"] as string,
                    NombreCompletoApellidos = row["NombreCompletoApellidos"] as string,
                    TipoID = row["TipoID"] as string,
                    NumeroID = row["NumeroID"] as string,
                    Telefono = row["Telefono"] as string,
                    Ciudad = row["Ciudad"] as string,
                    Direccion = row["Direccion"] as string,
                    Email = row["Email"] as string
                };
            }

            return objeto;
        }

        public async Task<bool> GuardaDatosRevisorFiscal(DatosRevisorFiscalDto objRegistro)
        {
            bool respuesta = false;
            try
            {
                if (ExisteDatosRevisorFiscal(objRegistro.Id, objRegistro.IdFormulario))
                {
                    respuesta = EditarDatosRevisorFiscal(objRegistro);
                }
                else
                {
                    respuesta = InsertarDatosRevisorFiscal(objRegistro);
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                throw new InvalidOperationException("Error al guardar/actualizar DatosRevisorFiscal", ex);
            }
            return respuesta;
        }


        public async Task<bool> CalcularRiesgoFormulario(int idFormulario)
        {
            try
            {
                var datosGenerales = await ConsultaDatosGenerales(idFormulario);
                if (datosGenerales == null)
                    return false;

                int categoriaTercero = datosGenerales.CategoriaTercero;
                int idPais = datosGenerales.Pais;

                var infoFinanciera = await ConsultaInformacionFinanciera(idFormulario);

                bool esPep = false;
                bool cotizaBolsa = false;
                int valorPersona = 0;
                valorPersona = await ObtenerValorCategoriaTercero(categoriaTercero);

                int valorPaisCorrup = 0;
                valorPaisCorrup = await ObtenerValorPaisRiesgo(idPais, "CorrupcionTI");

                int valorPaisGAFI = 0;
                valorPaisGAFI = await ObtenerValorPaisRiesgo(idPais, "GAFI");

                int valorPEP = 0;
                valorPEP = await ObtenerValorSiNo("PEP", esPep);

                int valorCotiza = 0;
                valorCotiza = await ObtenerValorSiNo("CotizaBolsa", cotizaBolsa);

                int valorTamano = 0;
                valorTamano = await ObtenerValorTamano(datosGenerales.TamanoTercero);

                bool manejaEfectivo = infoFinanciera != null && infoFinanciera.GrandesCantidadesEfectivo;
                int valorOperEfectivo = await ObtenerValorSiNo("OperacionesEfectivo", manejaEfectivo);

                bool manejaCripto = infoFinanciera != null && infoFinanciera.ActivosVirtuales;
                int valorCripto = await ObtenerValorSiNo("ActivosVirtuales", manejaCripto);


                int totalRiesgo = valorPersona + valorPaisCorrup + valorPaisGAFI
                                + valorPEP + valorCotiza + valorTamano
                                + valorOperEfectivo + valorCripto;


                string nivel;
                if (totalRiesgo >= 25) nivel = "Alto";
                else if (totalRiesgo >= 8) nivel = "Medio";
                else nivel = "Bajo";

                await UpsertFormularioRiesgoCalculado(idFormulario,
                                                     valorPersona,
                                                     valorPaisCorrup,
                                                     valorPaisGAFI,
                                                     valorPEP,
                                                     valorCotiza,
                                                     valorTamano,
                                                     valorOperEfectivo,
                                                     valorCripto,
                                                     totalRiesgo,
                                                     nivel);

                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error en CalcularRiesgoFormulario: " + ex.Message);

                throw;
            }
        }

        public async Task<int> ObtenerValorCategoriaTercero(int idCategoria)
        {
            int valor = 0;
            string strSQL = @"SELECT ValorNumerico 
                      FROM CategoriaTerceroRiesgo
                      WHERE IdCategoria = @IdCategoria";
            try
            {
                cDataBase.conectar();
                List<SqlParameter> param = new List<SqlParameter>() {
                    new SqlParameter("@IdCategoria", idCategoria)
                };
                DataTable dt = cDataBase.EjecutarConsultaConParametros(strSQL, param);
                cDataBase.desconectar();

                if (dt.Rows.Count > 0)
                    valor = Convert.ToInt32(dt.Rows[0]["ValorNumerico"]);
            }
            catch
            {
                cDataBase.desconectar();
            }
            return valor;
        }


        public async Task<int> ObtenerValorPaisRiesgo(int idPais, string dimension)
        {
            int valor = 0;
            string strSQL = @"SELECT ValorNumerico
                      FROM PaisRiesgo
                      WHERE IdPais=@IdPais
                        AND Dimension=@Dimension";
            try
            {
                cDataBase.conectar();
                List<SqlParameter> param = new List<SqlParameter>() {
            new SqlParameter("@IdPais", idPais),
            new SqlParameter("@Dimension", dimension)
        };
                DataTable dt = cDataBase.EjecutarConsultaConParametros(strSQL, param);
                cDataBase.desconectar();

                if (dt.Rows.Count > 0)
                    valor = Convert.ToInt32(dt.Rows[0]["ValorNumerico"]);
            }
            catch
            {
                cDataBase.desconectar();
            }
            return valor;
        }

        public async Task<int> ObtenerValorSiNo(string nombreDimension, bool opcion)
        {
            int valor = 0;
            string strSQL = @"SELECT ValorNumerico
                      FROM SiNoRiesgo
                      WHERE NombreDimension=@nombre
                        AND OpcionBit=@bit";
            try
            {
                cDataBase.conectar();
                List<SqlParameter> param = new List<SqlParameter>() {
            new SqlParameter("@nombre", nombreDimension),
            new SqlParameter("@bit", opcion)
        };
                DataTable dt = cDataBase.EjecutarConsultaConParametros(strSQL, param);
                cDataBase.desconectar();

                if (dt.Rows.Count > 0)
                    valor = Convert.ToInt32(dt.Rows[0]["ValorNumerico"]);
            }
            catch
            {
                cDataBase.desconectar();
            }
            return valor;
        }

        public async Task<int> ObtenerValorTamano(int idTamano)
        {
            int valor = 0;
            string strSQL = @"SELECT ValorNumerico
                      FROM TamanoTerceroRiesgo
                      WHERE IdTamano=@id";
            try
            {
                cDataBase.conectar();
                List<SqlParameter> param = new List<SqlParameter>() {
            new SqlParameter("@id", idTamano)
        };
                DataTable dt = cDataBase.EjecutarConsultaConParametros(strSQL, param);
                cDataBase.desconectar();

                if (dt.Rows.Count > 0)
                    valor = Convert.ToInt32(dt.Rows[0]["ValorNumerico"]);
            }
            catch
            {
                cDataBase.desconectar();
            }
            return valor;
        }

        public async Task UpsertFormularioRiesgoCalculado(int idFormulario, int valorPersona, int valorPaisCorrup, int valorPaisGafi, int valorPEP, int valorCotiza, int valorTamano, int valorOperEfectivo, int valorCripto, int totalRiesgo, string nivel)
        {

            string checkSQL = "SELECT Id FROM FormularioRiesgoCalculado WHERE IdFormulario=@IdF";
            int idRow = 0;
            try
            {
                cDataBase.conectar();
                List<SqlParameter> paramCheck = new List<SqlParameter>() {
            new SqlParameter("@IdF", idFormulario)
        };
                DataTable dt = cDataBase.EjecutarConsultaConParametros(checkSQL, paramCheck);
                if (dt.Rows.Count > 0)
                    idRow = Convert.ToInt32(dt.Rows[0]["Id"]);
                cDataBase.desconectar();
            }
            catch
            {
                cDataBase.desconectar();
            }

            if (idRow == 0)
            {
                string insertSQL = @"
            INSERT INTO FormularioRiesgoCalculado
            (
                IdFormulario,
                ValorPersona,
                ValorPaisCorrupcion,
                ValorPaisGAFI,
                ValorPEP,
                ValorCotizaBolsa,
                ValorTamano,
                ValorOperacionesEfectivo,
                ValorActivosVirtuales,
                TotalRiesgo,
                NivelRiesgoFinal,
                FechaCalculo
            )
            VALUES
            (
                @IdFormulario,
                @VPers,
                @VCorr,
                @VGafi,
                @Vpep,
                @Vbolsa,
                @Vtamano,
                @Vefectivo,
                @Vcripto,
                @Total,
                @Nivel,
                GETDATE()
            )";

                try
                {
                    cDataBase.conectar();
                    List<SqlParameter> paramInsert = new List<SqlParameter>()
            {
                new SqlParameter("@IdFormulario", idFormulario),
                new SqlParameter("@VPers", valorPersona),
                new SqlParameter("@VCorr", valorPaisCorrup),
                new SqlParameter("@VGafi", valorPaisGafi),
                new SqlParameter("@Vpep", valorPEP),
                new SqlParameter("@Vbolsa", valorCotiza),
                new SqlParameter("@Vtamano", valorTamano),
                new SqlParameter("@Vefectivo", valorOperEfectivo),
                new SqlParameter("@Vcripto", valorCripto),
                new SqlParameter("@Total", totalRiesgo),
                new SqlParameter("@Nivel", nivel)
            };
                    cDataBase.EjecutarSPParametrosSinRetornonuew(insertSQL, paramInsert);
                    cDataBase.desconectar();
                }
                catch
                {
                    cDataBase.desconectar();
                }
            }
            else
            {
                string updateSQL = @"
            UPDATE FormularioRiesgoCalculado
            SET
                ValorPersona=@VPers,
                ValorPaisCorrupcion=@VCorr,
                ValorPaisGAFI=@VGafi,
                ValorPEP=@Vpep,
                ValorCotizaBolsa=@Vbolsa,
                ValorTamano=@Vtamano,
                ValorOperacionesEfectivo=@Vefectivo,
                ValorActivosVirtuales=@Vcripto,
                TotalRiesgo=@Total,
                NivelRiesgoFinal=@Nivel,
                FechaCalculo=GETDATE()
            WHERE
                IdFormulario=@IdFormulario
        ";
                try
                {
                    cDataBase.conectar();
                    List<SqlParameter> paramUpdate = new List<SqlParameter>()
            {
                new SqlParameter("@IdFormulario", idFormulario),
                new SqlParameter("@VPers", valorPersona),
                new SqlParameter("@VCorr", valorPaisCorrup),
                new SqlParameter("@VGafi", valorPaisGafi),
                new SqlParameter("@Vpep", valorPEP),
                new SqlParameter("@Vbolsa", valorCotiza),
                new SqlParameter("@Vtamano", valorTamano),
                new SqlParameter("@Vefectivo", valorOperEfectivo),
                new SqlParameter("@Vcripto", valorCripto),
                new SqlParameter("@Total", totalRiesgo),
                new SqlParameter("@Nivel", nivel)
            };
                    cDataBase.EjecutarSPParametrosSinRetornonuew(updateSQL, paramUpdate);
                    cDataBase.desconectar();
                }
                catch
                {
                    cDataBase.desconectar();
                }
            }
        }
        private static T GetValueOrDefault<T>(DataRow row, string columnName, T defaultValue = default!)
        {
            if (!row.Table.Columns.Contains(columnName) || row.IsNull(columnName))
                return defaultValue;

            return (T)Convert.ChangeType(row[columnName], typeof(T));
        }
        public async Task<FormularioRiesgoCalculadoDto> ObtenerRiesgoFormulario(int IdFormulario)
        {
            string consulta = @"SELECT 
                                frc.[Id], 
                                frc.[IdFormulario], 
                                frc.[ValorPersona],
                                frc.[ValorPaisCorrupcion],
                                frc.[ValorPaisGAFI],
                                frc.[ValorPEP],
                                frc.[ValorCotizaBolsa],
                                frc.[ValorTamano],
                                frc.[ValorOperacionesEfectivo],
                                frc.[ValorActivosVirtuales],
                                frc.[TotalRiesgo],
                                frc.[NivelRiesgoFinal],
                                frc.[FechaCalculo],

                                p.Nombre_es AS NombrePais,
                                ct.Nombre_es AS CategoriaTercero,
                                tt.Nombre_es AS TamanoTercero,

                                CASE 
                                WHEN frc.ValorPaisCorrupcion > 0 THEN 'País con alto riesgo de corrupción según Transparencia Internacional'
                                ELSE 'País con bajo riesgo de corrupción'
                                END AS RazonPaisCorrupcion,

                                CASE 
                                WHEN frc.ValorPaisGAFI > 0 THEN 'País en lista GAFI de alto riesgo'
                                ELSE 'País no está en lista GAFI'
                                END AS RazonPaisGAFI,

                                CASE 
                                WHEN frc.ValorOperacionesEfectivo > 0 THEN 1 ELSE 0 END AS ManejaEfectivo,
                                CASE 
                                WHEN frc.ValorActivosVirtuales > 0 THEN 1 ELSE 0 END AS ManejaActivosVirtuales,
                                CASE 
                                WHEN frc.ValorPEP > 0 THEN 1 ELSE 0 END AS EsPEP,
                                CASE 
                                WHEN frc.ValorCotizaBolsa > 0 THEN 1 ELSE 0 END AS CotizaEnBolsa

                                FROM [dbo].[FormularioRiesgoCalculado] frc
                                INNER JOIN [dbo].[DatosGenerales] dg 
                                ON frc.IdFormulario = dg.IdFormulario
                                LEFT JOIN [dbo].[Paises] p 
                                ON dg.Pais = p.Id
                                LEFT JOIN [dbo].[CategoriaTercero] ct 
                                ON dg.CategoriaTercero = ct.Id
                                LEFT JOIN [dbo].[TamañoTercero] tt 
                                ON dg.TamanoTercero = tt.Id
                                WHERE frc.IdFormulario = @IdF";

            List<SqlParameter> parametros = new List<SqlParameter>() {
        new SqlParameter("@IdF", IdFormulario)
    };

            cDataBase.conectar();
            DataTable dt = cDataBase.EjecutarConsultaConParametros(consulta, parametros);
            cDataBase.desconectar();

            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];

            var dto = new FormularioRiesgoCalculadoDto
            {
                Id = GetValueOrDefault(row, "Id", 0),
                IdFormulario = GetValueOrDefault(row, "IdFormulario", 0),
                ValorPersona = GetValueOrDefault(row, "ValorPersona", 0),
                ValorPaisCorrupcion = GetValueOrDefault(row, "ValorPaisCorrupcion", 0),
                ValorPaisGAFI = GetValueOrDefault(row, "ValorPaisGAFI", 0),
                ValorPEP = GetValueOrDefault(row, "ValorPEP", 0),
                ValorCotizaBolsa = GetValueOrDefault(row, "ValorCotizaBolsa", 0),
                ValorTamano = GetValueOrDefault(row, "ValorTamano", 0),
                ValorOperacionesEfectivo = GetValueOrDefault(row, "ValorOperacionesEfectivo", 0),
                ValorActivosVirtuales = GetValueOrDefault(row, "ValorActivosVirtuales", 0),
                TotalRiesgo = GetValueOrDefault(row, "TotalRiesgo", 0),
                NivelRiesgoFinal = GetValueOrDefault(row, "NivelRiesgoFinal", string.Empty),
                FechaCalculo = GetValueOrDefault(row, "FechaCalculo", DateTime.MinValue),

                NombrePais = GetValueOrDefault(row, "NombrePais", "No especificado"),
                CategoriaTercero = GetValueOrDefault(row, "CategoriaTercero", "No especificado"),
                TamanoTercero = GetValueOrDefault(row, "TamanoTercero", "No especificado"),

                RazonPaisCorrupcion = GetValueOrDefault(row, "RazonPaisCorrupcion", "No aplica"),
                RazonPaisGAFI = GetValueOrDefault(row, "RazonPaisGAFI", "No aplica"),
                ManejaEfectivo = GetValueOrDefault(row, "ManejaEfectivo", false),
                ManejaActivosVirtuales = GetValueOrDefault(row, "ManejaActivosVirtuales", false),
                EsPEP = GetValueOrDefault(row, "EsPEP", false),
                CotizaEnBolsa = GetValueOrDefault(row, "CotizaEnBolsa", false)
            };

            return dto;
        }
    }

    }
