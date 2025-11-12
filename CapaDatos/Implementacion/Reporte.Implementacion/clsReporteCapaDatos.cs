using CapaDatos.Interfaz.Reporte.Interface;
using CapaDatos.util;
using CapaDTO.Peticiones;
using CapaDTO.ReportesDTO;
using CapaDTO.Respuestas;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos.Implementacion.Reporte.Implementacion
{
    public class clsReporteCapaDatos: IReporteCapaDatos
    {

        private cDataBase cDataBase;
        private readonly IConfiguration _configuration;

        public clsReporteCapaDatos(IConfiguration configuration)
        {
            _configuration = configuration;
            cDataBase = new cDataBase(_configuration);
        }
        public async Task<List<defaulchartsDto>> ReporteRegistroTiemposxarea(int IdUsuario)
        {
            List<defaulchartsDto> listaReporteXarea = new List<defaulchartsDto>();

            DataTable dtInformacion = ConsultaReporteRegistroTiemposxarea(IdUsuario);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    defaulchartsDto objlista = new defaulchartsDto();
                    objlista.NombreD= dtInformacion.Rows[rows]["NombreArea"].ToString();
                    objlista.NumeroHoras = Convert.ToDecimal(dtInformacion.Rows[rows]["TotalHoras"]);

                    listaReporteXarea.Add(objlista);
                }
            }
            return listaReporteXarea;

        }

        private DataTable ConsultaReporteRegistroTiemposxarea(int IdUsuario)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(string.Format("SELECT top 10 a.NombreArea, SUM(rt.NumeroHoras) AS TotalHoras FROM tbl_RegistroTiempo rt JOIN tbl_Area a ON rt.IdArea = a.Id where rt.IdArea in (select IdArea from [dbo].[UsuarioAreas] where IdUsuario={0}) AND MONTH(rt.FechaActividad) = MONTH(GETDATE())  AND YEAR(rt.FechaActividad) = YEAR(GETDATE())  GROUP BY a.NombreArea ORDER BY TotalHoras DESC;", IdUsuario));
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



        public async Task<List<defaulchartsDto>> ReporteRegistroTiemposxUsuario(int IdUsuario)
        {
            List<defaulchartsDto> listaReporteXarea = new List<defaulchartsDto>();

            DataTable dtInformacion = ConsultaReporteRegistroTiemposxUsuario(IdUsuario);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    defaulchartsDto objlista = new defaulchartsDto();
                    objlista.NombreD = dtInformacion.Rows[rows]["NombreUsuario"].ToString();
                    objlista.NumeroHoras = Convert.ToDecimal(dtInformacion.Rows[rows]["TotalHoras"]);

                    listaReporteXarea.Add(objlista);
                }
            }
            return listaReporteXarea;

        }

        private DataTable ConsultaReporteRegistroTiemposxUsuario(int IdUsuario)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(string.Format("SELECT top 10 u.Nombre + ' ' + u.Apellidos AS NombreUsuario, SUM(rt.NumeroHoras) AS TotalHoras FROM tbl_RegistroTiempo rt JOIN tbl_Usuarios u ON rt.IdUsuario = u.Id where rt.IdArea in (select IdArea from [dbo].[UsuarioAreas] where IdUsuario={0}) AND MONTH(rt.FechaActividad) = MONTH(GETDATE())  AND YEAR(rt.FechaActividad) = YEAR(GETDATE())   GROUP BY u.Nombre, u.Apellidos ORDER BY TotalHoras DESC;", IdUsuario));
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


         public async Task<List<defaulchartsDto>> ReporteRegistroTiemposxCliente(int IdUsuario)
        {
            List<defaulchartsDto> listaReporteXarea = new List<defaulchartsDto>();

            DataTable dtInformacion = ConsultaReporteRegistroTiemposxCliente(IdUsuario);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    defaulchartsDto objlista = new defaulchartsDto();
                    objlista.NombreD = dtInformacion.Rows[rows]["NombreCliente"].ToString();
                    objlista.NumeroHoras = Convert.ToDecimal(dtInformacion.Rows[rows]["TotalHoras"]);

                    listaReporteXarea.Add(objlista);
                }
            }
            return listaReporteXarea;

        }

        private DataTable ConsultaReporteRegistroTiemposxCliente(int IdUsuario)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(string.Format("SELECT top 10 c.Nombre AS NombreCliente, SUM(rt.NumeroHoras) AS TotalHoras FROM tbl_RegistroTiempo rt JOIN tbl_Cliente c ON rt.IdCliente = c.Id where rt.IdArea in (select IdArea from [dbo].[UsuarioAreas] where IdUsuario={0}) AND MONTH(rt.FechaActividad) = MONTH(GETDATE())  AND YEAR(rt.FechaActividad) = YEAR(GETDATE())  GROUP BY c.Nombre ORDER BY TotalHoras DESC;", IdUsuario));
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


        public async Task<List<defaulchartsDto>> ReporteRegistroTiemposxServicio(int IdUsuario)
        {
            List<defaulchartsDto> listaReporteXarea = new List<defaulchartsDto>();

            DataTable dtInformacion = ConsultaReporteRegistroTiemposxServicio(IdUsuario);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    defaulchartsDto objlista = new defaulchartsDto();
                    objlista.NombreD = dtInformacion.Rows[rows]["NombreServicio"].ToString();
                    objlista.NumeroHoras = Convert.ToDecimal(dtInformacion.Rows[rows]["TotalHoras"]);

                    listaReporteXarea.Add(objlista);
                }
            }
            return listaReporteXarea;

        }

        private DataTable ConsultaReporteRegistroTiemposxServicio(int IdUsuario)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(string.Format("SELECT top 10 s.NombreServicio, SUM(rt.NumeroHoras) AS TotalHoras FROM tbl_RegistroTiempo rt JOIN tbl_Servicios s ON rt.IdServicio = s.Id where rt.IdArea in (select IdArea from [dbo].[UsuarioAreas] where IdUsuario={0}) AND MONTH(rt.FechaActividad) = MONTH(GETDATE())  AND YEAR(rt.FechaActividad) = YEAR(GETDATE()) GROUP BY s.NombreServicio ORDER BY TotalHoras DESC;", IdUsuario));
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








        public async Task<List<FormularioDto>> ContultaInfobasicaFormularioList(ReporteDto objetofiltro)
        {
            List<FormularioDto> listaAreasDtos = new List<FormularioDto>();

            DataTable dtInformacion = ConsultainfoBasicaFormularios(objetofiltro);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    FormularioDto objlista = new FormularioDto();
                    objlista.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objlista.NombreUsuario = dtInformacion.Rows[rows]["IdUsuario"].ToString();
                    objlista.NombreUsuario = dtInformacion.Rows[rows]["NombreUsuario"].ToString().Trim();
                    objlista.IdUsuario = Convert.ToInt32(dtInformacion.Rows[rows]["IdUsuario"]);
                    objlista.IdEstado = Convert.ToInt32(dtInformacion.Rows[rows]["IdEstado"]);
                    objlista.Estado = dtInformacion.Rows[rows]["EstadoForm"].ToString().Trim();                   
                    objlista.FechaFormulario = dtInformacion.Rows[rows]["FechaFormulario"].ToString().Trim();                    
                    listaAreasDtos.Add(objlista);
                }
            }
            return listaAreasDtos;
        }




        //FormularioDto
        private DataTable ConsultainfoBasicaFormularios(ReporteDto objFiltro)
        {
            string where = "";


            if (objFiltro.IdUsuario != 0)
            {
                where = string.Format("Where  fcp.IdUsuario={0}", objFiltro.IdUsuario);
            }

            if ((objFiltro.IdEstado != 0))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.IdEstado={0}", objFiltro.IdEstado);
                }
                else
                {
                    where += string.Format(" And fcp.IdEstado={0}", objFiltro.IdEstado);

                }
            }

           

            if (!string.IsNullOrEmpty(objFiltro.FechaDe) && string.IsNullOrEmpty(objFiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario >= '{0}'", objFiltro.FechaDe + " 00:00:00");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario >= '{0}'", objFiltro.FechaDe + " 00:00:00");

                }
            }

            if (string.IsNullOrEmpty(objFiltro.FechaDe) && !string.IsNullOrEmpty(objFiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario <= '{0}'", objFiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario <= '{0}'", objFiltro.FechaHasta + " 23:59:59");

                }
            }

            if (!string.IsNullOrEmpty(objFiltro.FechaDe) && !string.IsNullOrEmpty(objFiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objFiltro.FechaDe + " 00:00:00", objFiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objFiltro.FechaDe + " 00:00:00", objFiltro.FechaHasta + " 23:59:59");

                }
            }


            DataTable dtInformacion = new DataTable();
            try
            {
                string Consulta = string.Format("SELECT fcp.Id,CONCAT(tblU.Nombre,' ',tblU.Apellidos) NombreUsuario,fcp.IdUsuario,fcp.IdEstado,tblEstadoForm.Nombre_es EstadoForm,fcp.FechaFormulario FROM [dbo].[FormularioClienteProveedores]  as fcp inner join [dbo].[tbl_Usuarios] as tblU on (tblU.Id=fcp.IdUsuario) inner join [dbo].[EstadoFormulario] as tblEstadoForm on (tblEstadoForm.Id=fcp.IdEstado) {0}", where);

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
        



        public async Task<List<DatosGeneralesReporteDto>> ConsultaDatosGenerales(ReporteDto objetofiltro)

        {

            string Consulta = string.Empty;

            string where = "";


            if (objetofiltro.IdUsuario != 0)
            {
                where = string.Format("Where  fcp.IdUsuario={0}", objetofiltro.IdUsuario);
            }

            if ((objetofiltro.IdEstado != 0))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.IdEstado={0}", objetofiltro.IdEstado);
                }
                else
                {
                    where += string.Format(" And fcp.IdEstado={0}", objetofiltro.IdEstado);

                }
            }



            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");

                }
            }

            if (string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");

                }
            }

            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");

                }
            }

            List<DatosGeneralesReporteDto> objetolsit = new List<DatosGeneralesReporteDto>();
           
            Consulta = string.Format("SELECT tblDG.[Id],tblDG.[IdFormulario],tblDG.[Empresa],tblTSolicitud.Nombre_es TipoSolicitud,tblTerc.Nombre_es ClaseTercero ,tblCatTer.Nombre_es CategoriaTercero,tblDG.[NombreRazonSocial], " +
                "tblTipDocon.Nombre_es TipoIdentificacion ,tblDG.[NumeroIdentificacion],tblDG.[DigitoVerificacion],tblPais.Nombre_es Pais ,tblDG.[Ciudad] ,tblTamTerc.Nombre_es TamanoTercero ,tblActEco.Nombre_es RazonSocial,tblDG.[DireccionPrincipal],tblDG.[CodigoPostal] ,tblDG.[CorreoElectronico],tblDG.[Telefono],Ofactu.Nombre_es ObligadoFacturarElectronicamente,tblDG.[CorreoElectronicoFacturaEletronica],sucOPais.Nombre_es SucursalOtroPais  ,tblDG.[OtroPais], tblDG.[JsonPreguntasPep] FROM [dbo].[DatosGenerales] as tblDG " +
                "inner join [dbo].[TipoSolicitud] as tblTSolicitud on (tblDG.TipoSolicitud=tblTSolicitud.Id) inner join [dbo].[ClaseTercero] as tblTerc on (tblDG.ClaseTercero=tblTerc.Id) inner join [dbo].[CategoriaTercero] as tblCatTer on (tblDG.CategoriaTercero=tblCatTer.Id) " +
                "inner join [dbo].[TipoDocumentos] as tblTipDocon on (tblDG.TipoIdentificacion=tblTipDocon.Id) inner join [dbo].[Paises] as tblPais  on (tblDG.Pais=tblPais.Id) left join [dbo].[TamañoTercero] as tblTamTerc  on (tblDG.TamanoTercero=tblTamTerc.Id) left join [dbo].[ActividadEconomicaCiiu] as tblActEco  on (tblDG.RazonSocial=tblActEco.Id) " +
                "inner join [dbo].[SINO] as Ofactu on (tblDG.ObligadoFacturarElectronicamente=Ofactu.Id) inner join [dbo].[SINO] as sucOPais on (tblDG.SucursalOtroPais=sucOPais.Id)" +
                "inner join [dbo].[FormularioClienteProveedores] AS fcp ON (fcp.Id=tblDG.IdFormulario) {0}",where);


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
                    DatosGeneralesReporteDto objeto = new DatosGeneralesReporteDto();
                    objeto.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objeto.IdFormulario = Convert.ToInt32(dtInformacion.Rows[rows]["IdFormulario"]);
                    objeto.FechaDiligenciamiento = "";
                    objeto.Empresa = dtInformacion.Rows[rows]["Empresa"].ToString();
                    objeto.TipoSolicitud = dtInformacion.Rows[rows]["TipoSolicitud"].ToString();
                    objeto.ClaseTercero = dtInformacion.Rows[rows]["ClaseTercero"].ToString();
                    objeto.CategoriaTercero =dtInformacion.Rows[rows]["CategoriaTercero"].ToString();                     
                    objeto.NombreRazonSocial = dtInformacion.Rows[rows]["NombreRazonSocial"].ToString();
                    objeto.TipoIdentificacion = dtInformacion.Rows[rows]["TipoIdentificacion"].ToString(); 
                    objeto.NumeroIdentificacion = dtInformacion.Rows[rows]["NumeroIdentificacion"].ToString();
                    objeto.DigitoVerificacion = dtInformacion.Rows[rows]["DigitoVerificacion"].ToString();
                    objeto.Pais = dtInformacion.Rows[rows]["Pais"].ToString(); 
                    objeto.Ciudad = dtInformacion.Rows[rows]["Ciudad"].ToString();
                    objeto.TamanoTercero = dtInformacion.Rows[rows]["TamanoTercero"].ToString(); 
                    objeto.ActividadEconimoca = dtInformacion.Rows[rows]["RazonSocial"].ToString();
                    objeto.DireccionPrincipal = dtInformacion.Rows[rows]["DireccionPrincipal"].ToString();
                    objeto.CodigoPostal = dtInformacion.Rows[rows]["CodigoPostal"].ToString();
                    objeto.CorreoElectronico = dtInformacion.Rows[rows]["CorreoElectronico"].ToString();
                    objeto.Telefono = dtInformacion.Rows[rows]["Telefono"].ToString();
                    objeto.ObligadoFE =dtInformacion.Rows[rows]["ObligadoFacturarElectronicamente"].ToString();
                    objeto.CorreoElectronicoFE = dtInformacion.Rows[rows]["CorreoElectronicoFacturaEletronica"].ToString();
                    objeto.TieneSucursalesOtrosPaises = dtInformacion.Rows[rows]["SucursalOtroPais"].ToString();
                    objeto.PaisesOtrasSucursales = dtInformacion.Rows[rows]["OtroPais"].ToString();
                    objeto.PreguntasAdicionales = dtInformacion.Rows[rows]["JsonPreguntasPep"];
                    objeto.EstadoCivil = dtInformacion.Rows[0]["EstadoCivil"].ToString();
                    objeto.ConyugeIdentificacion = dtInformacion.Rows[0]["ConyugeIdentificacion"].ToString();
                    var valorDb = dtInformacion.Rows[0]["tipoPago"];
                    objeto.tipoPago = valorDb == DBNull.Value ? false : Convert.ToBoolean(valorDb);

                    objetolsit.Add(objeto);

                }
                return objetolsit;
            }

            return null;

        }


        public async Task<List<RepJunAccDTO>> ConsultaInfoRepresentanteslegales(ReporteDto objetofiltro)

        {
            List <RepJunAccDTO> objetolsit = new List<RepJunAccDTO>();    
            string Consulta = string.Empty;
            object objetojson = null;
            string where = "";


            if (objetofiltro.IdUsuario != 0)
            {
                where = string.Format("Where  fcp.IdUsuario={0}", objetofiltro.IdUsuario);
            }

            if ((objetofiltro.IdEstado != 0))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.IdEstado={0}", objetofiltro.IdEstado);
                }
                else
                {
                    where += string.Format(" And fcp.IdEstado={0}", objetofiltro.IdEstado);

                }
            }



            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");

                }
            }

            if (string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");

                }
            }

            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");

                }
            }

            Consulta = string.Format("SELECT a.[Id] ,a.[IdFormulario] ,a.[JsonRepresentanteLegal]  FROM [dbo].[RepresentanteLegal] as a inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}",where);


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
                    RepJunAccDTO obj = new RepJunAccDTO();
                    obj.IdFomrulario= Convert.ToInt32(dtInformacion.Rows[rows]["IdFormulario"]);
                    obj.Persona = dtInformacion.Rows[rows]["JsonRepresentanteLegal"];
                    objetolsit.Add(obj);
                }                  
                return objetolsit;
            }

            return null;

        }


        public async Task<List<RepJunAccDTO>> ConsultaInfoJuntaDirectivalegales(ReporteDto objetofiltro)

        {
            List<RepJunAccDTO> objetolsit = new List<RepJunAccDTO>();
            string Consulta = string.Empty;
            object objetojson = null;


            string where = "";


            if (objetofiltro.IdUsuario != 0)
            {
                where = string.Format("Where  fcp.IdUsuario={0}", objetofiltro.IdUsuario);
            }

            if ((objetofiltro.IdEstado != 0))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.IdEstado={0}", objetofiltro.IdEstado);
                }
                else
                {
                    where += string.Format(" And fcp.IdEstado={0}", objetofiltro.IdEstado);

                }
            }



            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");

                }
            }

            if (string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");

                }
            }

            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");

                }
            }


            Consulta = string.Format("SELECT a.[Id] ,a.[IdFormulario] ,a.[JsonJuntaDirectiva]  FROM [dbo].[JuntaDirectiva] as a inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}",where);


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
                    RepJunAccDTO obj = new RepJunAccDTO();
                    obj.IdFomrulario = Convert.ToInt32(dtInformacion.Rows[rows]["IdFormulario"]);
                    obj.Persona = dtInformacion.Rows[rows]["JsonJuntaDirectiva"];
                    objetolsit.Add(obj);
                }
                return objetolsit;
            }

            return null;

        }


        public async Task<List<RepJunAccDTO>> ConsultaInfoAccionistas(ReporteDto objetofiltro)

        {
            List<RepJunAccDTO> objetolsit = new List<RepJunAccDTO>();
            string Consulta = string.Empty;
            object objetojson = null;
            string where = "";


            if (objetofiltro.IdUsuario != 0)
            {
                where = string.Format("Where  fcp.IdUsuario={0}", objetofiltro.IdUsuario);
            }

            if ((objetofiltro.IdEstado != 0))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.IdEstado={0}", objetofiltro.IdEstado);
                }
                else
                {
                    where += string.Format(" And fcp.IdEstado={0}", objetofiltro.IdEstado);

                }
            }



            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");

                }
            }

            if (string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");

                }
            }

            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");

                }
            }

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
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    RepJunAccDTO obj = new RepJunAccDTO();
                    obj.IdFomrulario = Convert.ToInt32(dtInformacion.Rows[rows]["IdFormulario"]);
                    obj.Persona = dtInformacion.Rows[rows]["JsonAccionistas"];
                    objetolsit.Add(obj);
                }
                return objetolsit;
            }

            return objetolsit;

        }


        public async Task<List<DatosContactoDto>> ListaDatosContacto(ReporteDto objetofiltro)
        {

            string Consulta = string.Empty;
            List<DatosContactoDto> listobjeto = new List<DatosContactoDto>();

            string where = "";


            if (objetofiltro.IdUsuario != 0)
            {
                where = string.Format("Where  fcp.IdUsuario={0}", objetofiltro.IdUsuario);
            }

            if ((objetofiltro.IdEstado != 0))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.IdEstado={0}", objetofiltro.IdEstado);
                }
                else
                {
                    where += string.Format(" And fcp.IdEstado={0}", objetofiltro.IdEstado);

                }
            }



            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");

                }
            }

            if (string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");

                }
            }

            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");

                }
            }

            Consulta = string.Format("SELECT a.[Id],a.[IdFormulario],a.[NombreContacto],a.[Cargo],a.[Area],a.[Telefono],a.[CorreoElectronico] FROM [dbo].[DatosContacto] as a inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}", where);


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
                    objDato.IdFormulario = Convert.ToInt32(dtInformacion.Rows[rows]["IdFormulario"]);
                    objDato.NombreContacto = dtInformacion.Rows[rows]["NombreContacto"].ToString().Trim();
                    objDato.CargoContacto = dtInformacion.Rows[rows]["Cargo"].ToString().Trim();
                    objDato.AreaContacto = dtInformacion.Rows[rows]["Area"].ToString().Trim();
                    objDato.TelefonoContacto = dtInformacion.Rows[rows]["Telefono"].ToString().Trim();
                    objDato.CorreoElectronico = dtInformacion.Rows[rows]["CorreoElectronico"].ToString().Trim();

                    listobjeto.Add(objDato);
                }


                return listobjeto;
            }

            return listobjeto;


        }

        public async Task<List<ReferenciaComercialesBancariasReporteDto>> ListaReferenciasComercialesBan(ReporteDto objetofiltro)
        {

            string Consulta = string.Empty;
            List<ReferenciaComercialesBancariasReporteDto> listobjeto = new List<ReferenciaComercialesBancariasReporteDto>();
            string where = "";


            if (objetofiltro.IdUsuario != 0)
            {
                where = string.Format("Where  fcp.IdUsuario={0}", objetofiltro.IdUsuario);
            }

            if ((objetofiltro.IdEstado != 0))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.IdEstado={0}", objetofiltro.IdEstado);
                }
                else
                {
                    where += string.Format(" And fcp.IdEstado={0}", objetofiltro.IdEstado);

                }
            }



            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");

                }
            }

            if (string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");

                }
            }

            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");

                }
            }



            Consulta = string.Format("SELECT  a.[IdFormulario] ,a.[IdFormulario]  ,b.Nombre_es TipoReferencia,a.[NombreCompleto] ,a.[Ciudad] ,a.[Telefono]  FROM [dbo].[ReferenciasComercialesBancarias] as a inner join [dbo].[TipoReferenciaComercial] as  b on (a.TipoReferencia=b.Id) inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}",where);


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
                    ReferenciaComercialesBancariasReporteDto objDato = new ReferenciaComercialesBancariasReporteDto();
                    //objDato.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objDato.IdFormulario = Convert.ToInt32(dtInformacion.Rows[rows]["IdFormulario"]);
                    objDato.TipoReferencia =dtInformacion.Rows[rows]["TipoReferencia"].ToString();
                    objDato.NombreCompleto = dtInformacion.Rows[rows]["NombreCompleto"].ToString().Trim();
                    objDato.Ciudad = dtInformacion.Rows[rows]["Ciudad"].ToString().Trim();
                    objDato.Telefono = dtInformacion.Rows[rows]["Telefono"].ToString().Trim();
                    listobjeto.Add(objDato);
                }


                return listobjeto;
            }

            return listobjeto;


        }


        public async Task<List<DatosPagosReporteDto>> ConsultaDatosPago(ReporteDto objetofiltro)
        {

            string Consulta = string.Empty;
            List<DatosPagosReporteDto> objlist = new List<DatosPagosReporteDto>();
            string where = "";


            if (objetofiltro.IdUsuario != 0)
            {
                where = string.Format("Where  fcp.IdUsuario={0}", objetofiltro.IdUsuario);
            }

            if ((objetofiltro.IdEstado != 0))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.IdEstado={0}", objetofiltro.IdEstado);
                }
                else
                {
                    where += string.Format(" And fcp.IdEstado={0}", objetofiltro.IdEstado);

                }
            }



            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");

                }
            }

            if (string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");

                }
            }

            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");

                }
            }
            Consulta = string.Format("SELECT a.[Id], a.[IdFormulario], a.[NombreBanco], a.[NumeroCuenta],b.Nombre_es TipoCuenta, a.[CodigoSwift], a.[Ciudad],c.Nombre_es Pais, a.[CorreoElectronico] FROM [dbo].[DatosDePagos] as a inner join [dbo].[TipoCuentaBanco] as b on (b.Id=a.TipoCuenta) inner join [dbo].[Paises] as c on (a.Pais=c.Id) inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}",where);
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
                    DatosPagosReporteDto objeto = new DatosPagosReporteDto();
                    objeto.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objeto.IdFormulario = Convert.ToInt32(dtInformacion.Rows[rows]["IdFormulario"]);
                    objeto.NombreBanco = dtInformacion.Rows[rows]["NombreBanco"].ToString();
                    objeto.NumeroCuenta = dtInformacion.Rows[rows]["NumeroCuenta"].ToString();
                    objeto.TipoCuenta = dtInformacion.Rows[rows]["TipoCuenta"].ToString();
                    objeto.CodigoSwift = dtInformacion.Rows[rows]["CodigoSwift"].ToString();
                    objeto.Ciudad = dtInformacion.Rows[rows]["Ciudad"].ToString();
                    objeto.Pais = dtInformacion.Rows[rows]["Pais"].ToString();
                    objeto.CorreoElectronico = dtInformacion.Rows[rows]["CorreoElectronico"].ToString();
                    objlist.Add(objeto);
                }
            }

            return objlist;

        }


        public async Task<List<DespachoMercanciaReporteDto>> ConsulataDespachoMercancia(ReporteDto objetofiltro)
        {

            string Consulta = string.Empty;
            List<DespachoMercanciaReporteDto> objList = new List<DespachoMercanciaReporteDto>();

            string where = "";

            if (objetofiltro.IdUsuario != 0)
            {
                where = string.Format("Where  fcp.IdUsuario={0}", objetofiltro.IdUsuario);
            }

            if ((objetofiltro.IdEstado != 0))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.IdEstado={0}", objetofiltro.IdEstado);
                }
                else
                {
                    where += string.Format(" And fcp.IdEstado={0}", objetofiltro.IdEstado);

                }
            }



            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");

                }
            }

            if (string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");

                }
            }

            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");

                }
            }

            Consulta = string.Format("SELECT a.[Id] ,a.[IdFormulario] ,a.[DireccionDespacho],b.Nombre_es Pais,a.[Cuidad] ,a.[CodigoPostalEnvio] ,a.[Telefono] FROM [dbo].[DespachoMercancia] as a inner join [dbo].[Paises] as b on(b.Id=a.Pais) inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}",where);
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
                    DespachoMercanciaReporteDto objeto = new DespachoMercanciaReporteDto();
                    objeto.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objeto.IdFormulario = Convert.ToInt32(dtInformacion.Rows[rows]["IdFormulario"]);
                    objeto.DireccionDespacho = dtInformacion.Rows[rows]["DireccionDespacho"].ToString();
                    objeto.Pais = dtInformacion.Rows[rows]["Pais"].ToString();
                    objeto.Cuidad = dtInformacion.Rows[rows]["Cuidad"].ToString();
                    objeto.CodigoPostalEnvio = dtInformacion.Rows[rows]["CodigoPostalEnvio"].ToString();
                    objeto.Telefono = dtInformacion.Rows[rows]["Telefono"].ToString();
                    objList.Add(objeto);
                }
                return objList;
            }
            return objList;

        }

        public async Task<List<CumplimientoNormativoDto>> ConsultaCumplimientoNormativo(ReporteDto objetofiltro)
        {
            List<CumplimientoNormativoDto> objlsita = new List<CumplimientoNormativoDto>();
            string where = "";

            if (objetofiltro.IdUsuario != 0)
            {
                where = string.Format("WHERE fcp.IdUsuario={0}", objetofiltro.IdUsuario);
            }

            if (objetofiltro.IdEstado != 0)
            {
                if (string.IsNullOrEmpty(where))
                    where = string.Format("WHERE fcp.IdEstado={0}", objetofiltro.IdEstado);
                else
                    where += string.Format(" AND fcp.IdEstado={0}", objetofiltro.IdEstado);
            }

            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                    where = string.Format("WHERE fcp.FechaFormulario >= '{0} 00:00:00'", objetofiltro.FechaDe);
                else
                    where += string.Format(" AND fcp.FechaFormulario >= '{0} 00:00:00'", objetofiltro.FechaDe);
            }

            if (string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                    where = string.Format("WHERE fcp.FechaFormulario <= '{0} 23:59:59'", objetofiltro.FechaHasta);
                else
                    where += string.Format(" AND fcp.FechaFormulario <= '{0} 23:59:59'", objetofiltro.FechaHasta);
            }

            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("WHERE fcp.FechaFormulario BETWEEN '{0} 00:00:00' AND '{1} 23:59:59'",
                        objetofiltro.FechaDe, objetofiltro.FechaHasta);
                }
                else
                {
                    where += string.Format(" AND fcp.FechaFormulario BETWEEN '{0} 00:00:00' AND '{1} 23:59:59'",
                        objetofiltro.FechaDe, objetofiltro.FechaHasta);
                }
            }

            string Consulta = string.Format(@"
                                                SELECT 
                                                    a.[id],
                                                    a.[id_formulario],
                                                    a.[sometida_sagrilaft],
                                                    a.[sometida_otro_sistema],
                                                    a.[adhesion_politicas_samsung],
                                                    a.[no_invest_sancion_laftfpadm],
                                                    a.[no_transacciones_ilicitas],
                                                    a.[acepta_monitoreo_info],
                                                    a.[no_listas_restrictivas],
                                                    a.[correo_reportar_incidentes]
                                                FROM [dbo].[CumplimientoNormativo] AS a
                                                INNER JOIN [dbo].[FormularioClienteProveedores] fcp 
                                                    ON (a.[id_formulario] = fcp.[Id])
                                                {0}", where);

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
                    DataRow dr = dtInformacion.Rows[rows];
                    CumplimientoNormativoDto objeto = new CumplimientoNormativoDto();
                    objeto.Id = Convert.ToInt32(dr["id"]);
                    objeto.IdFormulario = Convert.ToInt32(dr["id_formulario"]);
                    objeto.sometida_sagrilaft = Convert.ToBoolean(dr["sometida_sagrilaft"]);
                    objeto.sometida_otro_sistema = Convert.ToBoolean(dr["sometida_otro_sistema"]);
                    objeto.adhesion_politicas_samsung = Convert.ToBoolean(dr["adhesion_politicas_samsung"]);
                    objeto.no_invest_sancion_laftfpadm = Convert.ToBoolean(dr["no_invest_sancion_laftfpadm"]);
                    objeto.no_transacciones_ilicitas = Convert.ToBoolean(dr["no_transacciones_ilicitas"]);
                    objeto.acepta_monitoreo_info = Convert.ToBoolean(dr["acepta_monitoreo_info"]);
                    objeto.no_listas_restrictivas = Convert.ToBoolean(dr["no_listas_restrictivas"]);

                    if (dr["correo_reportar_incidentes"] != DBNull.Value)
                        objeto.correo_reportar_incidentes = dr["correo_reportar_incidentes"].ToString();
                    else
                        objeto.correo_reportar_incidentes = null;

                    objlsita.Add(objeto);
                }
            }

            return objlsita;
        }

        public async Task<List<ArchivoDto>> ConsultaInfoArchivoCargados(ReporteDto objetofiltro)
        {

            List<ArchivoDto> lstarchivo = new List<ArchivoDto>();
            DataTable dtInformacion = new DataTable();
            try
            {
                string where = "";

                if (objetofiltro.IdUsuario != 0)
                {
                    where = string.Format("Where  fcp.IdUsuario={0}", objetofiltro.IdUsuario);
                }

                if ((objetofiltro.IdEstado != 0))
                {
                    if (string.IsNullOrEmpty(where))
                    {
                        where = string.Format("Where  fcp.IdEstado={0}", objetofiltro.IdEstado);
                    }
                    else
                    {
                        where += string.Format(" And fcp.IdEstado={0}", objetofiltro.IdEstado);

                    }
                }



                if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && string.IsNullOrEmpty(objetofiltro.FechaHasta))
                {
                    if (string.IsNullOrEmpty(where))
                    {
                        where = string.Format("Where  fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");
                    }
                    else
                    {
                        where += string.Format(" And fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");

                    }
                }

                if (string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
                {
                    if (string.IsNullOrEmpty(where))
                    {
                        where = string.Format("Where  fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");
                    }
                    else
                    {
                        where += string.Format(" And fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");

                    }
                }

                if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
                {
                    if (string.IsNullOrEmpty(where))
                    {
                        where = string.Format("Where  fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");
                    }
                    else
                    {
                        where += string.Format(" And fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");

                    }
                }



                string strConsulta = string.Format("SELECT a.[Id], a.[NombreArchivo],a.[Extencion],a.[Peso] ,a.[Ubicacion],a.[Key],a.[IdFormulario]  FROM [dbo].[AdjuntoFormulario] as a  inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}",where);

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

            return lstarchivo;
        }


        public async Task<List<FormularioModelDTO>> ConsultaDatosInformacionOEA(ReporteDto objetofiltro)
        {
            string Consulta = string.Empty;
            List<FormularioModelDTO> objlist = new List<FormularioModelDTO>();
            string where = "";

            if (objetofiltro.IdUsuario != 0)
            {
                where = string.Format("Where  fcp.IdUsuario={0}", objetofiltro.IdUsuario);
            }

            if ((objetofiltro.IdEstado != 0))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.IdEstado={0}", objetofiltro.IdEstado);
                }
                else
                {
                    where += string.Format(" And fcp.IdEstado={0}", objetofiltro.IdEstado);

                }
            }



            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");

                }
            }

            if (string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");

                }
            }

            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");

                }
            }
            Consulta = string.Format("sELECT a.* FROM [dbo].[InformacionOEA] as a  inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}",where );
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
                    FormularioModelDTO objeto = new FormularioModelDTO();
                    objeto.IdFormulario = Convert.ToInt32(dtInformacion.Rows[rows]["IdFormulario"]);
                    objeto.Uen = dtInformacion.Rows[rows]["Uen"].ToString();
                    objeto.ResponsableVenta = dtInformacion.Rows[rows]["ResponsableVenta"].ToString();
                    objeto.CorreoElectronico = dtInformacion.Rows[rows]["CorreoElectronico"].ToString();
                    objeto.ResponsableCartera = dtInformacion.Rows[rows]["ResponsableCartera"].ToString();
                    objeto.ResponsableTecnico = dtInformacion.Rows[rows]["ResponsableTecnico"].ToString();
                    objeto.Moneda = dtInformacion.Rows[rows]["Moneda"].ToString();
                    objeto.FormaPago = dtInformacion.Rows[rows]["FormaPago"].ToString();
                    objeto.NumeroDias = Convert.ToInt32(dtInformacion.Rows[rows]["NumeroDias"]);
                    objeto.CadenaLogistica = dtInformacion.Rows[rows]["CadenaLogistica"].ToString();
                    objeto.ListasRiesgo = dtInformacion.Rows[rows]["ListasRiesgo"].ToString();
                    objeto.SustanciasNarcoticos = dtInformacion.Rows[rows]["SustanciasNarcoticos"].ToString();
                    objeto.Certificaciones = dtInformacion.Rows[rows]["Certificaciones"].ToString();
                    objeto.ProveedorCadenaLogistica = dtInformacion.Rows[rows]["ProveedorCadenaLogistica"].ToString();
                    objeto.RiesgoPais = dtInformacion.Rows[rows]["RiesgoPais"].ToString();
                    objeto.AntiguedadEmpresa = dtInformacion.Rows[rows]["AntiguedadEmpresa"].ToString();
                    objeto.RiesgoSeguridad = dtInformacion.Rows[rows]["RiesgoSeguridad"].ToString();
                    objeto.Valoracion = dtInformacion.Rows[rows]["Valoracion"].ToString();
                    objeto.ListasRiesgoCliente = dtInformacion.Rows[rows]["ListasRiesgoCliente"].ToString();
                    objeto.TipoNegociacion = dtInformacion.Rows[rows]["TipoNegociacion"].ToString();
                    objeto.VistoBuenoAseguradora = dtInformacion.Rows[rows]["VistoBuenoAseguradora"].ToString();
                    objeto.RiesgoPaisCliente = dtInformacion.Rows[rows]["RiesgoPaisCliente"].ToString();
                    objeto.CertificacionesInstitucionalidad = dtInformacion.Rows[rows]["CertificacionesInstitucionalidad"].ToString();
                    objeto.RiesgoSeguridadCliente = dtInformacion.Rows[rows]["RiesgoSeguridadCliente"].ToString();
                    objeto.ValoracionCliente = dtInformacion.Rows[rows]["ValoracionCliente"].ToString();
                    objeto.SegmentacionRiesgo = dtInformacion.Rows[rows]["SegmentacionRiesgo"].ToString();
                    objlist.Add(objeto);
                }
            }

            return objlist;

        }


        public async Task<List<InformacionTributariaDTO>> ConsultaInformacionTributaria(ReporteDto objetofiltro)

        {

            string Consulta = string.Empty;
            List<InformacionTributariaDTO> objetolist = new List<InformacionTributariaDTO>();

            string where = "";

            if (objetofiltro.IdUsuario != 0)
            {
                where = string.Format("Where  fcp.IdUsuario={0}", objetofiltro.IdUsuario);
            }

            if ((objetofiltro.IdEstado != 0))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.IdEstado={0}", objetofiltro.IdEstado);
                }
                else
                {
                    where += string.Format(" And fcp.IdEstado={0}", objetofiltro.IdEstado);

                }
            }



            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario >= '{0}'", objetofiltro.FechaDe + " 00:00:00");

                }
            }

            if (string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario <= '{0}'", objetofiltro.FechaHasta + " 23:59:59");

                }
            }

            if (!string.IsNullOrEmpty(objetofiltro.FechaDe) && !string.IsNullOrEmpty(objetofiltro.FechaHasta))
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = string.Format("Where  fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");
                }
                else
                {
                    where += string.Format(" And fcp.FechaFormulario BETWEEN '{0}' and '{1}'", objetofiltro.FechaDe + " 00:00:00", objetofiltro.FechaHasta + " 23:59:59");

                }
            }



            Consulta = string.Format("sELECT a.[Id],a.[IdFormulario] ,a.[GranContribuyente],a.[NumResolucionGranContribuyente] ,a.[FechaResolucionGranContribuyente],a.[Autorretenedor] ,a.[NumResolucionAutorretenedor],a.[FechaResolucionAutorretenedor] ,a.[ResponsableICA] ,a.[MunicipioRetener] ,a.[Tarifa] ,a.[ResponsableIVA] ,a.[AgenteRetenedorIVA]  FROM [dbo].[InformacionTributaria] as a inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}", where);


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

                    InformacionTributariaDTO objeto = new InformacionTributariaDTO();

                    objeto.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objeto.IdFormulario = Convert.ToInt32(dtInformacion.Rows[rows]["IdFormulario"]);
                    objeto.GranContribuyente = Convert.ToInt32(dtInformacion.Rows[rows]["GranContribuyente"]);
                    objeto.NumResolucionGranContribuyente = dtInformacion.Rows[rows]["NumResolucionGranContribuyente"].ToString();
                    objeto.FechaResolucionGranContribuyente = dtInformacion.Rows[rows]["FechaResolucionGranContribuyente"].ToString();
                    objeto.Autorretenedor = Convert.ToInt32(dtInformacion.Rows[rows]["Autorretenedor"]);
                    objeto.NumResolucionAutorretenedor = dtInformacion.Rows[rows]["NumResolucionAutorretenedor"].ToString();
                    objeto.FechaResolucionAutorretenedor = dtInformacion.Rows[rows]["FechaResolucionAutorretenedor"].ToString();
                    objeto.ResponsableICA = Convert.ToInt32(dtInformacion.Rows[rows]["ResponsableICA"]);
                    objeto.MunicipioRetener = dtInformacion.Rows[rows]["MunicipioRetener"].ToString();
                    objeto.Tarifa = Convert.ToInt32(dtInformacion.Rows[rows]["Tarifa"]);
                    objeto.ResponsableIVA = Convert.ToInt32(dtInformacion.Rows[rows]["ResponsableIVA"]);
                    objeto.AgenteRetenedorIVA = Convert.ToInt32(dtInformacion.Rows[rows]["AgenteRetenedorIVA"]);


                    objetolist.Add(objeto);
                }




                return objetolist;
            }

            return objetolist;

        }


    }
}
