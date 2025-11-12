using CapaDatos.Interfaz.Servicios.Interfaz;
using CapaDatos.util;
using CapaDTO.Peticiones;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos.Implementacion.Servicios.Implementacion
{
    public class clsServiciosCapaDatos : IServiciosCapaDatos
    {

        private cDataBase cDataBase;
        private readonly IConfiguration _configuration;

        public clsServiciosCapaDatos(IConfiguration configuration)
        {
            _configuration = configuration;
            cDataBase = new cDataBase(_configuration);
        }


        public async Task<List<ServiciosDto>> ListaServicios(int IdArea)
        {
            List<ServiciosDto> listaServiciosDtos = new List<ServiciosDto>();

            DataTable dtInformacion = ConsultaListaServicios(IdArea);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    ServiciosDto objlista = new ServiciosDto();
                    objlista.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objlista.NombreServicio = dtInformacion.Rows[rows]["NombreServicio"].ToString().Trim();
                    objlista.Descripcion = dtInformacion.Rows[rows]["Descripcion"].ToString().Trim();
                    objlista.Area= dtInformacion.Rows[rows]["NombreArea"].ToString().Trim();
                    objlista.IdArea = Convert.ToInt32(dtInformacion.Rows[rows]["IdArea"]);
                    objlista.Activo = Convert.ToBoolean(dtInformacion.Rows[rows]["Activo"]);
                    listaServiciosDtos.Add(objlista);
                }
            }
            return listaServiciosDtos;

        }


        public async Task<List<ServiciosDto>> ListaTodosServicios()
        {
            List<ServiciosDto> listaServiciosDtos = new List<ServiciosDto>();

            DataTable dtInformacion = ConsultaListaTodosServicios();
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    ServiciosDto objlista = new ServiciosDto();
                    objlista.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objlista.NombreServicio = dtInformacion.Rows[rows]["NombreServicio"].ToString().Trim();
                    objlista.Descripcion = dtInformacion.Rows[rows]["Descripcion"].ToString().Trim();
                    objlista.Area = dtInformacion.Rows[rows]["NombreArea"].ToString().Trim();
                    objlista.IdArea=Convert.ToInt32(dtInformacion.Rows[rows]["IdArea"]);
                    objlista.Activo = Convert.ToBoolean(dtInformacion.Rows[rows]["Activo"]);
                    listaServiciosDtos.Add(objlista);
                }
            }
            return listaServiciosDtos;

        }



        private DataTable ConsultaListaServicios(int IdArea)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta( string.Format("select SER.Id,SER.NombreServicio,SER.Descripcion,ARE.NombreArea,SER.IdArea,SER.Activo  FROM [dbo].[tbl_Servicios] SER inner join [dbo].[tbl_Area] ARE ON(SER.IdArea=ARE.Id) where SER.IdArea={0} and Activo=1 order by NombreServicio asc", IdArea));
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

        private DataTable ConsultaListaTodosServicios()
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(string.Format("select SER.Id,SER.NombreServicio,SER.Descripcion,ARE.NombreArea,SER.IdArea, SER.Activo  FROM [dbo].[tbl_Servicios] SER inner join [dbo].[tbl_Area] ARE ON(SER.IdArea=ARE.Id) order by NombreServicio asc"));
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


        public async Task<bool> CrearServicio(ServiciosDto servicio)
        {
            string strConsulta = string.Empty;
            bool respuesta = false;

            try
            {
                strConsulta = string.Format("insert into [dbo].[tbl_Servicios] values ('{0}','{1}',{2},1)", servicio.NombreServicio, servicio.Descripcion, servicio.IdArea);

                cDataBase.conectar();
                cDataBase.ejecutarQuery(strConsulta);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");

            }

            return respuesta;
        }

        public async Task<bool> EditarServicio(ServiciosDto servicio)
        {
            string strConsulta = string.Empty;
            bool respuesta = false;

            try
            {
                strConsulta = string.Format("update [dbo].[tbl_Servicios]  set NombreServicio='{0}', Descripcion='{1}' ,IdArea={3},Activo={4} where Id={2}", servicio.NombreServicio, servicio.Descripcion, servicio.Id,servicio.IdArea, Convert.ToSByte(servicio.Activo));

                cDataBase.conectar();
                cDataBase.ejecutarQuery(strConsulta);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el Cliente");

            }

            return respuesta;
        }


        public async Task<bool> EliminarServicio(int IdServcio)
        {
            string strConsulta = string.Empty;
            bool respuesta = false;

            try
            {
                strConsulta = string.Format("Delete from [dbo].[tbl_Servicios] where Id={0} ", IdServcio);
                cDataBase.conectar();
                cDataBase.ejecutarQuery(strConsulta);
                respuesta = true;
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Eliminar el Servicio");
            }
            return respuesta;
        }

    }
}
