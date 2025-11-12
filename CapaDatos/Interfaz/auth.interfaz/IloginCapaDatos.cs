using CapaDTO.Peticiones;
using CapaDTO.Respuestas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos.Interfaz.auth.interfaz
{
    public interface IloginCapaDatos
    {

        Task<DataTable> autenticarUsuario(string Usuario, string Contrasena);

        Task<CurrentUserInfo> Isadmin(string username);

        Task<int> iduser(string username);

        Task<string> Cambiodepassword(string password, string usuario, string newpassword);

        Task<string> Updtepassword(string usuario, string newpassword);


        Task<bool> CrearUsuario(UsuarioDto objRegistro);

        Task<List<UsuariolistDto>> ListaUsuariosapp();

        Task<bool> EditarUsuario(UsuarioDto objRegistro);

        Task<DataTable> DatosRecuperacion(string NumeroDocumento, string Email);

        Task<bool> CambiodepasswordAleatorio(int IdUsuario, string NuevoPass);

        Task<List<UsuariolistDto>> ListaUsuariosappmisareas(int IdUsuario);

        Task<string> DevuelveCorreoCompradorVendedor(int IdUsuario);
    }
}
