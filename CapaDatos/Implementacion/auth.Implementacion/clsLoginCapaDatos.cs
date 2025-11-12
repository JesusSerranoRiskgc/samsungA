using CapaDatos.Interfaz.auth.interfaz;
using CapaDatos.util;
using CapaDTO.Peticiones;
using CapaDTO.Respuestas;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using static System.Formats.Asn1.AsnWriter;

namespace CapaDatos.Implementacion.auth.Implementacion
{
    public class clsLoginCapaDatos : IloginCapaDatos
    {

        private cEncriptacion cEncrypt = new cEncriptacion();
        private cDataBase cDataBase;
        private readonly IConfiguration _configuration;


        public clsLoginCapaDatos(IConfiguration configuration)
        {
            _configuration = configuration;
            cDataBase = new cDataBase(configuration);
        }

        public async Task<DataTable> autenticarUsuario(string Usuario, string Contrasena)
        {
            DataTable dtInformacion = new DataTable();
            string strConsulta = string.Empty, strEncryptPass = string.Empty;

            try
            {
                strEncryptPass = cEncriptacion.CifradoData(Contrasena);
                
                // ✅ MODIFICACIÓN: Permitir login con Usuario, Identificacion o Email
                strConsulta = string.Format(
                    "SELECT Id,Nombre,Apellidos FROM [dbo].[tbl_Usuarios] " +
                    "WHERE (Usuario = '{0}' OR Identificacion = '{0}' OR Email = '{0}') " +
                    "AND Password = '{1}' AND Activo = 1",
                    Usuario, strEncryptPass);
            
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

            return dtInformacion;
        }


        public async Task<DataTable> DatosRecuperacion(string NumeroDocumento, string Email)
        {

            DataTable dtInformacion = new DataTable();
            string strConsulta = string.Empty, strEncryptPass = string.Empty;

            try
            {
                // ✅ MODIFICACIÓN: Permitir búsqueda flexible por Usuario, Identificacion o Email
                strConsulta = string.Format(
                    "SELECT * FROM [dbo].[tbl_Usuarios] " +
                    "WHERE (Usuario='{0}' OR Identificacion='{0}') AND Email='{1}' AND Activo=1",
                    NumeroDocumento, Email);
            
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

            return dtInformacion;
        }



        public async Task<CurrentUserInfo> Isadmin(string username)
        {
            DataTable dtInformacion = new DataTable();
            string strConsulta;
            try
            {
                // ✅ MODIFICACIÓN: Buscar por Usuario, Identificacion o Email
                strConsulta = string.Format(
                    "SELECT u.id,u.Nombre,u.Apellidos,u.Email,u.Identificacion,b.Nombre as Rol,u.ActualizarPass " +
                    "FROM tbl_Usuarios u " +
                    "INNER JOIN [dbo].[tbl_TipoUsuario] b ON (u.TipoUsuario=b.Id) " +
                    "WHERE (u.Usuario='{0}' OR u.Identificacion='{0}' OR u.Email='{0}') " +
                    "GROUP BY u.Id, u.Nombre, u.Apellidos,u.Email,u.Identificacion,b.Nombre,u.IdArea,u.ActualizarPass", 
                    username);
            
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
                CurrentUserInfo userinfo = new CurrentUserInfo();
                userinfo.Nombre = dtInformacion.Rows[0]["Nombre"].ToString().Trim();
                userinfo.Apellido = dtInformacion.Rows[0]["Apellidos"].ToString().Trim();
                // ✅ MODIFICACIÓN: Usar Identificacion como usuario principal
                userinfo.Usuario = dtInformacion.Rows[0]["Identificacion"].ToString().Trim();
                userinfo.Rol = dtInformacion.Rows[0]["Rol"].ToString().Trim();               
                userinfo.ActualizarPass = string.IsNullOrEmpty(dtInformacion.Rows[0]["ActualizarPass"].ToString()) ? true : Convert.ToBoolean(dtInformacion.Rows[0]["ActualizarPass"]) ;

                return userinfo;

            }
            else
            {
                return null;
            }

        }

        public async Task<int> iduser(string username)
        {
            DataTable dtInformacion = new DataTable();
            string strConsulta;
            try
            {
                // ✅ MODIFICACIÓN: Buscar por Usuario, Identificacion o Email
                strConsulta = string.Format(
                    "SELECT Id FROM [dbo].[tbl_Usuarios] " +
                    "WHERE (Usuario='{0}' OR Identificacion='{0}' OR Email='{0}')", 
                    username);
            
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
                int idtipouser = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                return idtipouser;
            }

            return 0;

        }

        public async Task<string> Cambiodepassword(string password, string usuario, string newpassword)
        {
            string strMensaje = string.Empty;
            if (mtdCompararContrasenasEncriptadas(password, contrasenaUsuario(usuario)))
            {
                if (!mtdValidarContrasena(newpassword, ref strMensaje))
                {
                    return strMensaje;
                }
                else if (!mtdCompararContrasenasEncriptadas(newpassword, contrasenaUsuario(usuario)))
                {
                    actualizarContrasena(newpassword, await iduser(usuario),0);
                    return "Contraseña cambiada con éxito.";
                }
                else
                {
                    return "Cambio de contraseña sin éxito. Tu nueva contraseña no puede ser igual a tu antigua contraseña.";
                }

                return "";
            }
            else
            {

                return "Cambio de contraseña sin éxito. Has escrito mal tu antigua contraseña.";
            }
        }


        public async Task<bool> CambiodepasswordAleatorio(int IdUsuario, string NuevoPass)
        {

            try
            {
           
                actualizarContrasena(NuevoPass, IdUsuario, 1);
                return true;
            }catch (Exception  ex)
            {
                return false;
            }

                  
                    
              
        }




        public async Task<string> Updtepassword(string usuario, string newpassword)
        {
            string strMensaje = string.Empty;

                if (!mtdValidarContrasena(newpassword, ref strMensaje))
                {
                    return strMensaje;
                }
                else if (!mtdCompararContrasenasEncriptadas(newpassword, contrasenaUsuario(usuario)))
                {
                    actualizarContrasena(newpassword, await iduser(usuario),0);
                    return "Contraseña cambiada con éxito.";
                }
                else
                {
                throw new InvalidOperationException("Cambio de contraseña sin éxito. Tu nueva contraseña no puede ser igual a tu antigua contraseña.");
               
                }

                return "";
            
        }



        private void actualizarContrasena(String Contrasena, int idusuario,int actualizapass)
        {
            string strConsulta = string.Empty, strPassEncrypted = string.Empty;

            try
            {
                strPassEncrypted = cEncriptacion.CifradoData(Contrasena);//cEncriptacion.Base64_Encode(Contrasena); //mtdEncriptarContrasena(Contrasena);
                strConsulta = "UPDATE [dbo].[tbl_Usuarios] SET Password = N'" + strPassEncrypted + "' , ActualizarPass="+ actualizapass + " WHERE (Id = " + idusuario + ")";
                cDataBase.conectar();
                cDataBase.ejecutarQuery(strConsulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();

                //throw new Exception(ex.Message);
            }
        }

        public bool mtdValidarContrasena(string strContrasena, ref string strMensaje)
        {
            #region Variables
            bool booResult = true;
            string strRegExpr = @"(?=^.{8,}$)(?=.*?[0-9]*)(?=.*[!-/:-@{-~Ç-■^_])(?=.*[A-Z]+)(?=.*[a-z]*).*$";
            Regex regExpresion = new Regex(strRegExpr);
            #endregion

            #region Validacion Longitudes
            if (strContrasena.Length < 8)
            {
                booResult = false;
                strMensaje = "La contraseña es menor a 8 caracteres.";
            }
            #endregion Validacion Longitudes

            #region Validacion de caracteres
            if (booResult)
            {
                if (!regExpresion.IsMatch(strContrasena))
                {
                    booResult = false;
                    strMensaje = "La contraseña debe tener al menos una letra mayúscula y un caracter especial, letras minúsculas y/o números, sin espacios.";
                }
            }
            #endregion Validacion de caracteres

            return booResult;
        }
        private String contrasenaUsuario(string Usuario)
        {
            DataTable dtInformacion = new DataTable();
            string strResult = string.Empty;

            try
            {
                cDataBase.conectar();
                // ✅ MODIFICACIÓN: Buscar por Usuario, Identificacion o Email
                string strConsulta = string.Format(
                    "SELECT LTRIM(RTRIM(Password)) AS Contrasena " +
                    "FROM [dbo].[tbl_Usuarios] " +
                    "WHERE (Usuario = '{0}' OR Identificacion = '{0}' OR Email = '{0}')", 
                    Usuario);
            
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
                strResult = dtInformacion.Rows[0]["Contrasena"].ToString().Trim();
            }

            return strResult;
        }


        private bool mtdCompararContrasenasEncriptadas(string strPassUser, string strPassBD)
        {
            #region Variables
            bool booResult = true;
            string strPassUserCrypt = string.Empty;
            #endregion Variables

            if (strPassUser != strPassBD)
            {
                strPassUserCrypt = cEncriptacion.CifradoData(strPassUser); //cEncriptacion.Base64_Encode(strPassUser); //mtdEncriptarContrasena(strPassUser);

                /*if (strPassUserCrypt != strPassBD)*/
                int intValue = string.Compare(strPassUserCrypt, strPassBD);

                if (intValue != 0)
                    booResult = false;
            }
            return booResult;
        }

        public async Task<bool> CrearUsuario(UsuarioDto objRegistro)
        {
            string strConsulta = string.Empty;
            bool respuesta = false;
            string strTemporalPASS = string.Empty;
            DataTable dtInformacion = new DataTable();
            bool vienePassword = !string.IsNullOrEmpty(objRegistro.Password);

            strTemporalPASS = cEncriptacion.CifradoData(objRegistro.Password);
            int actualizarPass;
            string Usuario = string.Empty;
            try
            {
                if (vienePassword)
                {
                    strTemporalPASS = cEncriptacion.CifradoData(objRegistro.Password);
                    actualizarPass = 0;  
                }
                else
                {
                    strTemporalPASS = cEncriptacion.CifradoData("ENK4R1$SK");
                    actualizarPass = 1;  
                }

                // ✅ CORRECCIÓN: Usar Identificacion como nombre de usuario
                string usuario = objRegistro.Identificacion;

                int scope = 0;
                strConsulta = string.Format(
                    "INSERT INTO [dbo].[tbl_Usuarios] " +
                    "([Nombre], [Apellidos], [Email], [Password], [TipoUsuario], [FechaCreacion], [Identificacion], [Activo], [ActualizarPass], [Usuario], [IdCompradorVendedor]) " +
                    "VALUES ('{0}', '{1}', '{2}', '{3}', {4}, GETDATE(), '{5}', 1, {6}, '{7}', {8}) SELECT SCOPE_IDENTITY()",
                    objRegistro.Nombres,
                    objRegistro.Apellidos,
                    objRegistro.Email,
                    strTemporalPASS,
                    objRegistro.IdTipoUsuario,
                    objRegistro.Identificacion,
                    actualizarPass,
                    usuario, // ✅ Ahora usa Identificacion
                    objRegistro.idCompradorVendedor
                );

                dtInformacion = cDataBase.mtdEjecutarConsultaSQL(strConsulta);
                if (dtInformacion.Rows.Count > 0)
                {
                    scope = Convert.ToInt32(dtInformacion.Rows[0][0]);
                }

                respuesta = true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("error al crear el Cliente");
            }

            return respuesta;
        }

        private void GuardaAreaUsuario(int idUsuario, List<int> Listaarea)

        {

            try
            {
                string strConsulta = string.Empty;
                foreach (int Area in Listaarea)
                {
                    strConsulta = string.Format("Insert into UsuarioAreas values ({0},{1})", idUsuario,Area);

                    cDataBase.conectar();
                    cDataBase.ejecutarQuery(strConsulta);
                    cDataBase.desconectar();
                }       
            
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
            }

          }


        public async Task<List<UsuariolistDto>> ListaUsuariosapp()
        {
            List<UsuariolistDto> listaUsuariosDtos = new List<UsuariolistDto>();

            DataTable dtInformacion = ConsultaRegistroUsuarios();
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    UsuariolistDto objlista = new UsuariolistDto();
                    objlista.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objlista.Nombres = dtInformacion.Rows[rows]["Nombre"].ToString().Trim();
                    objlista.Apellidos = dtInformacion.Rows[rows]["Apellidos"].ToString().Trim();                    
                    objlista.Email = dtInformacion.Rows[rows]["Email"].ToString().Trim();
                    objlista.Identificacion =dtInformacion.Rows[rows]["Identificacion"].ToString().Trim();
                    objlista.Usuario = dtInformacion.Rows[rows]["Usuario"].ToString().Trim();
                    objlista.IdTipoUsuario = Convert.ToInt32(dtInformacion.Rows[rows]["TipoUsuario"]);
                    objlista.NombreTipoUsuario = dtInformacion.Rows[rows]["NombreTipoUsuario"].ToString().Trim();
                    objlista.IdCompradorVendedor= string.IsNullOrEmpty(dtInformacion.Rows[rows]["IdCompradorVendedor"].ToString()) ? -1 : Convert.ToInt32(dtInformacion.Rows[rows]["IdCompradorVendedor"]);
                    objlista.CorreoCompradorVendedor = dtInformacion.Rows[rows]["CorreoCompradorVendedor"].ToString().Trim();
                    objlista.Activo = Convert.ToBoolean(dtInformacion.Rows[rows]["Activo"]);
                   
                    listaUsuariosDtos.Add(objlista);
                }
            }
            return listaUsuariosDtos;

        }


        public async Task<List<UsuariolistDto>> ListaUsuariosappmisareas(int IdUsuario)
        {
            List<UsuariolistDto> listaUsuariosDtos = new List<UsuariolistDto>();

            DataTable dtInformacion = ConsultaRegistroUsuariosigualareas(IdUsuario);
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    UsuariolistDto objlista = new UsuariolistDto();
                    objlista.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objlista.Nombres = dtInformacion.Rows[rows]["Nombre"].ToString().Trim();
                    objlista.Apellidos = dtInformacion.Rows[rows]["Apellidos"].ToString().Trim();
                    objlista.Email = dtInformacion.Rows[rows]["Email"].ToString().Trim();
                    objlista.IdTipoUsuario = Convert.ToInt32(dtInformacion.Rows[rows]["TipoUsuario"]);
                    objlista.Identificacion = dtInformacion.Rows[rows]["Identificacion"].ToString().Trim();
                    objlista.Activo = Convert.ToBoolean(dtInformacion.Rows[rows]["Activo"]);
                   

                    listaUsuariosDtos.Add(objlista);
                }
            }
            return listaUsuariosDtos;

        }

        private List<AreaDto> ListaAreasUsuario(int idUsuario)
        {
            List<AreaDto> listaAreas = new List<AreaDto>();
            DataTable dtInformacion = new DataTable();
            dtInformacion = ConsultaAreasxusuario(idUsuario);

            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    AreaDto objarea = new AreaDto();

                    objarea.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objarea.NombreArea = dtInformacion.Rows[rows]["NombreArea"].ToString();
                    objarea.Descripcion= dtInformacion.Rows[rows]["Descripcion"].ToString();
                    listaAreas.Add(objarea);
                }
            }
            return listaAreas;

        }

        private DataTable ConsultaAreasxusuario(int IdUsuario)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta("select A.Id,A.NombreArea,A.Descripcion  from [dbo].[tbl_Area] as A INNER JOIN UsuarioAreas AS B ON A.Id=B.IdArea WHERE B.IdUsuario="+IdUsuario+" ORDER BY A.Id asc");
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

        private DataTable ConsultaRegistroUsuarios()
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta("select distinct us.Id, us.Nombre,us.Apellidos,us.Identificacion,us.Email,us.Usuario, us.TipoUsuario,TU.Nombre as NombreTipoUsuario,us.Activo,us1.Id as IdCompradorVendedor, us1.Email AS CorreoCompradorVendedor from [dbo].[tbl_Usuarios] as us  LEFT JOIN [dbo].[tbl_Usuarios] AS US1 ON (US.IdCompradorVendedor=US1.id) inner JOIN [dbo].[tbl_TipoUsuario] AS TU ON (TU.Id=us.TipoUsuario)");
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

        private DataTable ConsultaRegistroUsuariosigualareas(int idUsuario)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta("SELECT distinct a.[Id],a.[Nombre],a.[Apellidos],a.[Email],a.[Password],a.[TipoUsuario],a.[FechaCreacion],a.[Identificacion],a.[IdArea],a.[Activo],a.[ActualizarPass],a.[PermisoRegistroCualquierFecha],a.[PermitirRegistrosCuarquierHora],a.[PermitirMasHoras] FROM [dbo].[tbl_Usuarios] as a inner join [dbo].[UsuarioAreas] as b on(a.Id=b.IdUsuario) where b.IdArea in (select IdArea from [dbo].[UsuarioAreas]  where IdUsuario=" + idUsuario +")");
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



        public async Task<bool> EditarUsuario(UsuarioDto objRegistro)
        {
            string strConsulta = string.Empty;
            bool respuesta = false;
            string strTemporalPASS = string.Empty;

            strTemporalPASS = cEncriptacion.CifradoData(objRegistro.Identificacion);

            try
            {

                strConsulta = string.Format("UPDATE [dbo].[tbl_Usuarios] SET " +
                           "Nombre = '{0}', " +
                           "Apellidos = '{1}', " +                          
                           "TipoUsuario = {2}, " +                          
                           "Identificacion = '{3}', " +
                           "[Email]='{4}', "+
                           "[IdCompradorVendedor]={5}, "+
                           "Activo = {6} " +                          
                           "WHERE Id = {7}",
                           objRegistro.Nombres,
                           objRegistro.Apellidos,
                           objRegistro.IdTipoUsuario,
                           objRegistro.Identificacion,
                           objRegistro.Email,
                           objRegistro.idCompradorVendedor, // Asumiendo que strTemporalPASS es la contraseña encriptada
                           Convert.ToByte(objRegistro.Activo),
                           objRegistro.Id
                           );

                cDataBase.conectar();
                cDataBase.ejecutarQuery(strConsulta);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al actualizar el Cliente");

            }

            return respuesta;
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

    }
}
