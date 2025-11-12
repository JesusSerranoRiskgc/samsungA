using CapaDatos.Interfaz.Listas.interfaz;
using CapaDatos.util;
using CapaDTO.Peticiones;
using CapaDTO.Respuestas;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos.Implementacion.Listas.Implementacion
{
    public class clsListasCombos : IListasCombosCapaDatos
    {
        private cDataBase cDataBase;
        private readonly IConfiguration _configuration;

        public clsListasCombos(IConfiguration configuration)
        {
            _configuration = configuration;
            cDataBase = new cDataBase(_configuration);
        }
        public async Task<List<SeleccionDto>> ListaSiNo(string Lang)
        {
            List<SeleccionDto> listaAreasDtos = new List<SeleccionDto>();

            DataTable dtInformacion = ConsultaListaSino(Lang);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    SeleccionDto objlista = new SeleccionDto();
                    objlista.Id = dtInformacion.Rows[rows]["Id"].ToString();                    
                    objlista.Nombre = dtInformacion.Rows[rows]["Nombre"].ToString().Trim();

                    listaAreasDtos.Add(objlista);
                }
            }
            return listaAreasDtos;

        }

        public async Task<List<SeleccionDto>> TipoUsuario()
        {
            List<SeleccionDto> listaAreasDtos = new List<SeleccionDto>();

            DataTable dtInformacion = ConsultaTipoUsuario();
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    SeleccionDto objlista = new SeleccionDto();
                    objlista.Id = dtInformacion.Rows[rows]["Id"].ToString();
                    objlista.Nombre = dtInformacion.Rows[rows]["Nombre"].ToString().Trim();

                    listaAreasDtos.Add(objlista);
                }
            }
            return listaAreasDtos;

        }

        private DataTable ConsultaTipoUsuario()
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string Consulta = string.Format("SELECT Id, Nombre as Nombre FROM [dbo].[tbl_TipoUsuario]");
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

        private DataTable ConsultaListaSino(string lang)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string Consulta = string.Format("SELECT Id, Nombre_{0} as Nombre FROM [dbo].[SINO] order by Id desc",lang);
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

        public async Task<List<SeleccionDto>> ConsultaTipoSolicitud(string Lang)
        {
            List<SeleccionDto> listaAreasDtos = new List<SeleccionDto>();

            DataTable dtInformacion = TipoSolicitud(Lang);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    SeleccionDto objlista = new SeleccionDto();
                    objlista.Id = dtInformacion.Rows[rows]["Id"].ToString();
                    objlista.Nombre = dtInformacion.Rows[rows]["Nombre"].ToString().Trim();

                    listaAreasDtos.Add(objlista);
                }
            }
            return listaAreasDtos;

        }

        private DataTable TipoSolicitud(string lang)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string Consulta = string.Format("SELECT Id, Nombre_{0} as Nombre FROM [dbo].[TipoSolicitud]", lang);
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


        public async Task<List<SeleccionDto>> ClaseTercero(string Lang)
        {
            List<SeleccionDto> listaAreasDtos = new List<SeleccionDto>();

            DataTable dtInformacion = ConsultaClaseTercero(Lang);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    SeleccionDto objlista = new SeleccionDto();
                    objlista.Id = dtInformacion.Rows[rows]["Id"].ToString();
                    objlista.Nombre = dtInformacion.Rows[rows]["Nombre"].ToString().Trim();

                    listaAreasDtos.Add(objlista);
                }
            }
            return listaAreasDtos;

        }

        private DataTable ConsultaClaseTercero(string lang)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string Consulta = string.Format("SELECT Id, Nombre_{0} as Nombre FROM [dbo].[ClaseTercero]", lang);
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

        public async Task<List<SeleccionDto>> CategoriaTercero(string Lang)
        {
            List<SeleccionDto> listaAreasDtos = new List<SeleccionDto>();

            DataTable dtInformacion = ConsultaCategoriaTercero(Lang);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    SeleccionDto objlista = new SeleccionDto();
                    objlista.Id = dtInformacion.Rows[rows]["Id"].ToString();
                    objlista.Nombre = dtInformacion.Rows[rows]["Nombre"].ToString().Trim();

                    listaAreasDtos.Add(objlista);
                }
            }
            return listaAreasDtos;

        }

        private DataTable ConsultaCategoriaTercero(string lang)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string Consulta = string.Format("SELECT Id, Nombre_{0} as Nombre FROM [dbo].[CategoriaTercero]", lang);
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


        public async Task<List<SeleccionDto>> Paises(string Lang)
        {
            List<SeleccionDto> listaAreasDtos = new List<SeleccionDto>();

            DataTable dtInformacion = ConsultaPaises(Lang);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    SeleccionDto objlista = new SeleccionDto();
                    objlista.Id = dtInformacion.Rows[rows]["Id"].ToString();
                    objlista.Nombre = dtInformacion.Rows[rows]["Nombre"].ToString().Trim();

                    listaAreasDtos.Add(objlista);
                }
            }
            return listaAreasDtos;

        }

        private DataTable ConsultaPaises(string lang)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string Consulta = string.Format("SELECT Id, Nombre_{0} as Nombre FROM [dbo].[Paises]", lang);
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

        public async Task<List<SeleccionDto>> TamañoTercero (string Lang)
        {
            List<SeleccionDto> listaAreasDtos = new List<SeleccionDto>();

            DataTable dtInformacion = ConsultaTamañoTercero(Lang);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    SeleccionDto objlista = new SeleccionDto();
                    objlista.Id = dtInformacion.Rows[rows]["Id"].ToString();
                    objlista.Nombre = dtInformacion.Rows[rows]["Nombre"].ToString().Trim();

                    listaAreasDtos.Add(objlista);
                }
            }
            return listaAreasDtos;

        }

        private DataTable ConsultaTamañoTercero(string lang)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string Consulta = string.Format("SELECT Id, Nombre_{0} as Nombre FROM [dbo].[TamañoTercero]", lang);
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

        public async Task<List<SeleccionDto>> ActividadEconomica(string Lang)
        {
            List<SeleccionDto> listaAreasDtos = new List<SeleccionDto>();

            DataTable dtInformacion = ConsultaActividadEconomica(Lang);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    SeleccionDto objlista = new SeleccionDto();
                    objlista.Id = dtInformacion.Rows[rows]["Id"].ToString();
                    objlista.Nombre = dtInformacion.Rows[rows]["Nombre"].ToString().Trim();

                    listaAreasDtos.Add(objlista);
                }
            }
            return listaAreasDtos;

        }

        private DataTable ConsultaActividadEconomica(string lang)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string Consulta = string.Format("SELECT Id, CONCAT (CodigoCIIU ,'. ',Nombre_{0}) as Nombre FROM [dbo].[ActividadEconomicaCiiu]  ", lang);
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

        public async Task<List<SeleccionDto>> TiposDocumentos(string Lang)
        {
            List<SeleccionDto> listaAreasDtos = new List<SeleccionDto>();

            DataTable dtInformacion = ConsultaTiposDocumentos(Lang);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    SeleccionDto objlista = new SeleccionDto();
                    objlista.Id = dtInformacion.Rows[rows]["Id"].ToString();
                    objlista.Nombre = dtInformacion.Rows[rows]["Nombre"].ToString().Trim();

                    listaAreasDtos.Add(objlista);
                }
            }
            return listaAreasDtos;

        }

        private DataTable ConsultaTiposDocumentos(string lang)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string Consulta = string.Format("SELECT Id, Nombre_{0} as Nombre FROM [dbo].[TipoDocumentos]  ", lang);
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

        public async Task<List<SeleccionDto>> listaTipoCuentaBancaria(string Lang)
        {
            List<SeleccionDto> listaAreasDtos = new List<SeleccionDto>();

            DataTable dtInformacion = ConsultaTipoCuentaBancaria(Lang);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    SeleccionDto objlista = new SeleccionDto();
                    objlista.Id = dtInformacion.Rows[rows]["Id"].ToString();
                    objlista.Nombre = dtInformacion.Rows[rows]["Nombre"].ToString().Trim();

                    listaAreasDtos.Add(objlista);
                }
            }
            return listaAreasDtos;

        }

        private DataTable ConsultaTipoCuentaBancaria(string lang)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string Consulta = string.Format("SELECT Id, Nombre_{0} as Nombre FROM [dbo].[TipoCuentaBanco]  ", lang);
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

        public async Task<List<SeleccionDto>> listaTipoReferenciaBanCom(string Lang)
        {
            List<SeleccionDto> listaAreasDtos = new List<SeleccionDto>();

            DataTable dtInformacion = ConsultaTipoReferenciaBanCom(Lang);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    SeleccionDto objlista = new SeleccionDto();
                    objlista.Id = dtInformacion.Rows[rows]["Id"].ToString();
                    objlista.Nombre = dtInformacion.Rows[rows]["Nombre"].ToString().Trim();

                    listaAreasDtos.Add(objlista);
                }
            }
            return listaAreasDtos;

        }      

        private DataTable ConsultaTipoReferenciaBanCom(string lang)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string Consulta = string.Format("SELECT Id, Nombre_{0} as Nombre FROM [dbo].[TipoReferenciaComercial] ", lang);
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

        public async Task<List<SeleccionDto>> ListaEmpeadosCompradoresVendedores()
        {
            List<SeleccionDto> listaAreasDtos = new List<SeleccionDto>();

            DataTable dtInformacion = ConsultaListaUsuarios();
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    SeleccionDto objlista = new SeleccionDto();
                    objlista.Id = dtInformacion.Rows[rows]["Id"].ToString();
                    objlista.Nombre = dtInformacion.Rows[rows]["Nombre"].ToString().Trim();

                    listaAreasDtos.Add(objlista);
                }
            }
            return listaAreasDtos;

        }

        private DataTable ConsultaListaUsuarios()
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string Consulta = string.Format("select Id,  Email, CONCAT(Nombre, '', Apellidos) as Nombre  from [dbo].[tbl_Usuarios] where TipoUsuario in (1,2)");
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


        public async Task<List<SeleccionDto>> ConsultaUsuariosProveedorCliente()
        {
            List<SeleccionDto> listaAreasDtos = new List<SeleccionDto>();

            DataTable dtInformacion = ConsultaUserClientePro();
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    SeleccionDto objlista = new SeleccionDto();
                    objlista.Id = dtInformacion.Rows[rows]["Id"].ToString();
                    objlista.Nombre = String.Concat(dtInformacion.Rows[rows]["Nombre"].ToString().Trim(),' ', dtInformacion.Rows[rows]["Apellidos"].ToString().Trim());

                    listaAreasDtos.Add(objlista);
                }
            }
            return listaAreasDtos;

        }

        private DataTable ConsultaUserClientePro()
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string Consulta = string.Format("SELECT * From [dbo].[tbl_Usuarios] where TipoUsuario=7");
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


        public async Task<List<SeleccionDto>> ConsultaEstadosFormularioConstulta()
        {
            List<SeleccionDto> listaAreasDtos = new List<SeleccionDto>();

            DataTable dtInformacion = ConsultaEstadosFormularios();
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    SeleccionDto objlista = new SeleccionDto();
                    objlista.Id = dtInformacion.Rows[rows]["Id"].ToString();
                    objlista.Nombre = dtInformacion.Rows[rows]["Nombre_es"].ToString().Trim();

                    listaAreasDtos.Add(objlista);
                }
            }
            return listaAreasDtos;

        }

        private DataTable ConsultaEstadosFormularios()
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string Consulta = string.Format("SELECT [Id] ,[Nombre_es] ,[Nombre_en] FROM [FCC_ENKA_Pruebas].[dbo].[EstadoFormulario] where id >2");
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


    }
}
