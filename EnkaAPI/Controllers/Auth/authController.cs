using CapaDTO.Peticiones;
using CapaDTO.Respuestas;
using CapaNegocio.Interfaz.auth.interfaz;
using EnkaAPI.jtw;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnkaAPI.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class authController : ControllerBase
    {
        protected readonly IloginCapaNegocios _iloginCapaNegocios;
        private readonly IConfiguration _configuration;
        private readonly TokenGenerator _itogengenerator;


        public authController(IloginCapaNegocios iloginCapaNegocios, TokenGenerator itogengenerator, IConfiguration configuration)
        {
            this._iloginCapaNegocios = iloginCapaNegocios;
            _configuration = configuration;
            _itogengenerator = itogengenerator;
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Authenticate(LoginRequest login)
        {
            if (login == null)
            {
                return BadRequest(ModelState);
            }
            DataTable dtInformacion = await _iloginCapaNegocios.autenticarUsuario(login.email, login.password);
            if (dtInformacion.Rows.Count > 0)
            {
                var token = _itogengenerator.GenerateTokenJwt(login, dtInformacion);
                return Ok(new { token = token });
            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("Recuperacion")]
        public async Task<IActionResult> Recuperacion(RecuperacionDto objRecuperacion)
        {
            if (objRecuperacion == null)
            {
                return BadRequest(new { error = "Datos inválidos" });
            }

            try
            {
                bool Respuesta = await _iloginCapaNegocios.DatosRecuperacion(objRecuperacion.Usuario, objRecuperacion.CorreoElectronico);

                if (Respuesta)
                {
                    return Ok(new { message = "Se ha enviado un correo con la nueva contraseña" });
                }
                else
                {
                    return BadRequest(new { error = "No se pudo procesar la recuperación de contraseña" });
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Error No se encuentra usuario")
                {
                    return NotFound(new { error = "No se encontró un usuario con los datos proporcionados" });
                }
                
                return BadRequest(new { error = "Ocurrió un error al procesar la solicitud" });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("current")]
        public async Task<IActionResult> userinformartion2()
        {
            string UserName = "";

            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;

            }
            else
            {
                return Unauthorized();
            }

            CurrentUserInfo userinfo = new CurrentUserInfo();
            userinfo = await _iloginCapaNegocios.Isadmin(UserName);
            return Ok(userinfo);
        }

        [HttpPost]
        [Authorize]
        [Route("passwordchange")]
        public async Task<IActionResult> changepassword(PasswordChangeModel changepassword)
        {
            string UserName = "";

            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;

            }
            else
            {
                return BadRequest();
            }

            string RESPUESTA = await _iloginCapaNegocios.Cambiodepassword(changepassword.password, UserName, changepassword.newpassword);

            return Ok(new
            {
                Respuesta = RESPUESTA
            });

        }

        [HttpPost]
        [Authorize]
        [Route("passwordUpdate")]
        public async Task<IActionResult> passwordUpdate(PasswordUpdateModel changepassword)
        {
            string UserName = "";

            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;

            }
            else
            {
                return BadRequest();
            }

            try
            {
                string RESPUESTA = await _iloginCapaNegocios.Updtepassword(UserName, changepassword.newpassword);

                return Ok(new
                {
                    Respuesta = RESPUESTA
                });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }



        }

        [HttpPost]
        [AllowAnonymous]
        [Route("addnewuser")]
        public async Task<IActionResult> addnewuser(UsuarioDto objRegistro)
        {
            string UserName = "";

            try
            {
                bool respuesta = await _iloginCapaNegocios.CrearUsuario(objRegistro);

                return Ok(respuesta);

            }
            catch (Exception ex)
            {
                return BadRequest();

            }
        }


        [HttpPost]
        [Authorize]
        [Route("edituser")]
        public async Task<IActionResult> edituser(UsuarioDto objRegistro)
        {
            string UserName = "";

            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;

            }
            else
            {
                return Unauthorized();
            }

            try
            {
                bool respuesta = await _iloginCapaNegocios.EditarUsuario(objRegistro);

                return Ok(respuesta);

            }
            catch (Exception ex)
            {
                return BadRequest();

            }
        }


        [HttpGet]
        [Authorize]
        [Route("listaUsuariosAPP")]
        public async Task<IActionResult> listaUsuariosAPP()
        {

            string UserName = "";
            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;

            }
            else
            {
                return BadRequest();
            }

            try
            {
                List<UsuariolistDto> lstUsuariosapp = new List<UsuariolistDto>();
                {
                    lstUsuariosapp = await _iloginCapaNegocios.ListaUsuariosapp();
                }

                return Ok(lstUsuariosapp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }



       

    }
}
