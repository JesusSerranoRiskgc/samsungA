using CapaDatos.Interfaz.auth.interfaz;
using CapaDTO.Common;
using CapaDTO.Peticiones;
using CapaDTO.Respuestas;
using CapaNegocio.Interfaz.auth.interfaz;
using CapaNegocio.Interfaz.Email.Interfaz;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Implementacion.auth.implementacion
{
    public class clsLoginCapaNegocios : IloginCapaNegocios
    {
        protected readonly IloginCapaDatos ilogincapadatos;
        private readonly IConfiguration _configuration;
        protected readonly IEMailService _iEMailService;

        public clsLoginCapaNegocios(IloginCapaDatos ilogincapadatos, IConfiguration configuration, IEMailService iEMailService)
        {
            this.ilogincapadatos = ilogincapadatos;
            _configuration = configuration;
            _iEMailService = iEMailService;
        }


        public async Task<DataTable> autenticarUsuario(string Usuario, string Contrasena)

        {
            return await ilogincapadatos.autenticarUsuario(Usuario, Contrasena);

        }

        public async Task<CurrentUserInfo> Isadmin(string username)
        {
            return await ilogincapadatos.Isadmin(username);
        }

        public async Task<int> iduser(string username)
        {
            return await ilogincapadatos.iduser(username);
        }

        public async Task<string> Cambiodepassword(string password, string usuario, string newpassword)
        {
            return await ilogincapadatos.Cambiodepassword(password, usuario, newpassword);
        }
        public async Task<string> Updtepassword(string usuario, string newpassword)
        {
            return await ilogincapadatos.Updtepassword(usuario, newpassword);
        }

        public async Task<bool> CrearUsuario(UsuarioDto objRegistro) {
            bool resultadoCreacion = await ilogincapadatos.CrearUsuario(objRegistro);

            if (resultadoCreacion)
            {
                Task.Run(() => EnviarNotificacionCreaccionUsuarioAsync(objRegistro));
            }

            return resultadoCreacion;


        }

        public async Task<List<UsuariolistDto>> ListaUsuariosapp() {
            return await ilogincapadatos.ListaUsuariosapp();
        }

        public async Task<bool> EditarUsuario(UsuarioDto objRegistro)
        {
            return await ilogincapadatos.EditarUsuario(objRegistro);
        }


        public async Task<bool> DatosRecuperacion(string Usuario, string Email)
        {
            DataTable datosUsuario= await ilogincapadatos.DatosRecuperacion(Usuario, Email);


            if (datosUsuario.Rows.Count > 0)
            {

                string Nuevapassword = mtdCrearRandomPassword(10);
                bool cambiocontraseña = await ilogincapadatos.CambiodepasswordAleatorio(Convert.ToInt32(datosUsuario.Rows[0]["Id"]), Nuevapassword);

                if (cambiocontraseña)
                {
                    EmailMessageRequestDto objetomail = new EmailMessageRequestDto();
                    List<string> destinatarios = new List<string>();
                    destinatarios.Add(Email);


                    objetomail.To = destinatarios;
                    objetomail.Subject = "Restauracion De Contraseña ";
                    objetomail.Body = $@"
    <html>
    <body style='font-family: Arial, sans-serif;'>
        <h2>Su contraseña ha sido restablecida</h2>
        <p>Por favor, ingrese con su nueva contraseña:</p>
        <p><strong>{Nuevapassword}</strong></p>
        <p>Gracias.</p>
    </body>
    </html>
";
                    try
                    {
                        var corro = await _iEMailService.SendMail(objetomail);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("error al enviar correo");
                    }

                }

            }
            else
            {
                throw new Exception("Error No se encuentra usuario");
            }

            return false;
        }


        private string mtdCrearRandomPassword(int PasswordLength)
        {
            string _allowedChars = "ASDFGHJKLÑQWERTYUIOPZXCVBNMasdfghjklñqwertyuiopzxcvbnm0123456789";
            Byte[] randomBytes = new Byte[PasswordLength];
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;

            for (int i = 0; i < PasswordLength; i++)
            {
                Random randomObj = new Random();
                randomObj.NextBytes(randomBytes);
                chars[i] = _allowedChars[(int)randomBytes[i] % allowedCharCount];
            }

            return new string(chars);
        }
        public async Task<List<UsuariolistDto>> ListaUsuariosappmisareas(int IdUsuario) {
            return await ilogincapadatos.ListaUsuariosappmisareas(IdUsuario);
        }


        private async Task EnviarNotificacionCreaccionUsuarioAsync(UsuarioDto objRegistro)
        {
            string CorreoVendedorComprador = string.Empty;
            string Usuario = string.Empty;
            EmailMessageRequestDto objetomail = new EmailMessageRequestDto();
            List<string> destinatarios = new List<string>();

            if (objRegistro.IdTipoUsuario == 7)
            {
                CorreoVendedorComprador = await ilogincapadatos.DevuelveCorreoCompradorVendedor(objRegistro.idCompradorVendedor);
                destinatarios.Add(CorreoVendedorComprador);
                Usuario = objRegistro.Identificacion;
            }
            else {
                Usuario = objRegistro.Email;
            }
            destinatarios.Add(objRegistro.Email);

            string cuerpoCorreo = @"
    <!DOCTYPE html>
    <html>
    <head>
        <style>
            .email-body {
                font-family: Arial, sans-serif;
                line-height: 1.6;
                color: #333;
            }
            .email-header {
                background-color: #1139d6;
                color: white;
                padding: 10px;
                text-align: center;
            }
            .email-content {
                padding: 20px;
            }
            .email-footer {
                background-color: #f1f1f1;
                padding: 10px;
                text-align: center;
                color: #555;
            }
            .highlight {
                color: #1139d6;
                font-weight: bold;
            }
        </style>
    </head>
    <body class='email-body'>
        <div class='email-header'>
            <h1>Creación de Usuario en la Plataforma</h1>
        </div>
        <div class='email-content'>
            <p>Se ha creado con éxito el siguiente usuario para registro en la plataforma:</p>
            <p>Tercero: <b>{Nombrecompleto}</b></p>
            <p>Identificación: <b>{Identificacion}</b></p>
            <p>Correo electrónico: <b>{CorreoElectronico}</b></p>
            <p>Contacto: <b>{CorreoVendedorComprador}</b></p>
            <p>Por favor ingrese al aplicativo <span class='highlight'>{plataforma}</span> su contraseña es :{contrasena}</p>
            <p>Muchas gracias.</p>
        </div>
        <div class='email-footer'>
            <p>&copy; 2024 Enka. Todos los derechos reservados.</p>
        </div>
    </body>
    </html>";

            cuerpoCorreo = cuerpoCorreo.Replace("{usuario}", Usuario)
                           .Replace("{plataforma}", "https://ambientetest.datalaft.com:2198/")
                           .Replace("{contrasena}", "ENK4R1$SK").Replace("{Nombrecompleto}", string.Concat(objRegistro.Nombres, " ", objRegistro.Apellidos))
                           .Replace("{Identificacion}", objRegistro.Identificacion).Replace("{CorreoElectronico}", objRegistro.Email).Replace("{CorreoVendedorComprador}", CorreoVendedorComprador);

            string subject = "Alerta de Inscripción -{RazonSocial}";

            subject = subject.Replace("{RazonSocial}", string.Concat(objRegistro.Nombres, " ", objRegistro.Apellidos));


            objetomail.To = destinatarios;
            objetomail.Subject = subject;
            objetomail.Subject.Replace("{RazonSocial}", string.Concat(objRegistro.Nombres, " ", objRegistro.Apellidos));
            objetomail.Body = cuerpoCorreo;


            try
            {
                var corro = await _iEMailService.SendMail(objetomail);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("error al enviar correo");
            }
        }
    }
}
