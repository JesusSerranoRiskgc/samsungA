using CapaDTO.Peticiones;
using CapaDTO.Respuestas;
using CapaNegocio.Interfaz.Area.Interfaz;
using CapaNegocio.Interfaz.auth.interfaz;
using CapaNegocio.Interfaz.Listas.Interfaz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EnkaAPI.Controllers.Combos
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListasSeleccionController : ControllerBase
    {

        protected readonly IListasCapaNegocios _IListasCapaNegocios;
        private readonly IConfiguration _configuration;
       

        public ListasSeleccionController( IConfiguration _configuration, IListasCapaNegocios IListasCapaNegocios)
        {
            this._IListasCapaNegocios = IListasCapaNegocios;
            this._configuration = _configuration;
        }

        [HttpGet]
        [Authorize]
        [Route("listaSiNO")]
        public async Task<IActionResult> listaSiNO(string Lang)
        {
            List<SeleccionDto> lstSiNO = new List<SeleccionDto>();

            lstSiNO = await _IListasCapaNegocios.ListaSiNo(Lang);

            return Ok(lstSiNO);
        }


        [HttpGet]
        [Authorize]
        [Route("listaTipoUsuario")]
        public async Task<IActionResult> listaTipoUsuario()
        {
            List<SeleccionDto> lstSiNO = new List<SeleccionDto>();

            lstSiNO = await _IListasCapaNegocios.TipoUsuario();

            return Ok(lstSiNO);
        }




        [HttpGet]
        [Authorize]
        [Route("listaTipoSolicitud")]
        public async Task<IActionResult> listaTipoSolicitud(string Lang)
        {
            List<SeleccionDto> lstTipoSolicitud = new List<SeleccionDto>();

            lstTipoSolicitud = await _IListasCapaNegocios.ConsultaTipoSolicitud(Lang);

            return Ok(lstTipoSolicitud);
        }



        [HttpGet]
        [Authorize]
        [Route("listaClaseTercero")]
        public async Task<IActionResult> listaClaseTercero(string Lang)
        {
            List<SeleccionDto> lstClaseTercero = new List<SeleccionDto>();

            lstClaseTercero = await _IListasCapaNegocios.ClaseTercero(Lang);

            return Ok(lstClaseTercero);
        }

        [HttpGet]
        [Authorize]
        [Route("listaCategoriaTercero")]
        public async Task<IActionResult> listaCategoriaTercero(string Lang)
        {
            List<SeleccionDto> lstCategoriaTercero = new List<SeleccionDto>();

            lstCategoriaTercero = await _IListasCapaNegocios.CategoriaTercero(Lang);

            return Ok(lstCategoriaTercero);
        }

        [HttpGet]
        [Authorize]
        [Route("listaPaises")]
        public async Task<IActionResult> listalistaPaises(string Lang)
        {
            List<SeleccionDto> lstlistaPaises = new List<SeleccionDto>();

            lstlistaPaises = await _IListasCapaNegocios.Paises(Lang);

            return Ok(lstlistaPaises);
        }

        [HttpGet]
        [Authorize]
        [Route("listaTamañoTercero")]
        public async Task<IActionResult> listaTamañoTercero(string Lang)
        {
            List<SeleccionDto> lstTamañoTercero = new List<SeleccionDto>();

            lstTamañoTercero = await _IListasCapaNegocios.TamañoTercero(Lang);

            return Ok(lstTamañoTercero);
        }

        [HttpGet]
        [Authorize]
        [Route("listaActividadEconomica")]
        public async Task<IActionResult> listaActividadEconomica(string Lang)
        {
            List<SeleccionDto> lstActividadEconomica = new List<SeleccionDto>();

            lstActividadEconomica = await _IListasCapaNegocios.ActividadEconomica(Lang);

            return Ok(lstActividadEconomica);
        }

        [HttpGet]
        [Authorize]
        [Route("listaTiposDocumentos")]
        public async Task<IActionResult> listaTiposDocumentos(string Lang)
        {
            List<SeleccionDto> lstTiposDocumentos = new List<SeleccionDto>();


            lstTiposDocumentos = await _IListasCapaNegocios.TiposDocumentos(Lang);

            return Ok(lstTiposDocumentos);
        }

        [HttpGet]
        [Route("listaTiposDocumentos2")]
        public async Task<IActionResult> listaTiposDocumentos2(string Lang)
        {
            List<SeleccionDto> lstTiposDocumentos = new List<SeleccionDto>();


            lstTiposDocumentos = await _IListasCapaNegocios.TiposDocumentos(Lang);

            return Ok(lstTiposDocumentos);
        }


        [HttpGet]
        [Authorize]
        [Route("listaTipoCuentaBancaria")]
        public async Task<IActionResult> listaTipoCuentaBancaria(string Lang)
        {
            List<SeleccionDto> lstTiposDocumentos = new List<SeleccionDto>();


            lstTiposDocumentos = await _IListasCapaNegocios.listaTipoCuentaBancaria(Lang);

            return Ok(lstTiposDocumentos);
        }

        [HttpGet]
        [Authorize]
        [Route("listaTipoReferenciaBanCom")]
        public async Task<IActionResult> listaTipoReferenciaBanCom(string Lang)
        {
            List<SeleccionDto> lstTipoReferencia = new List<SeleccionDto>();


            lstTipoReferencia = await _IListasCapaNegocios.listaTipoReferenciaBanCom(Lang);

            return Ok(lstTipoReferencia);
        }

        [HttpGet]
        [Authorize]
        [Route("listaEmpleados")]
        public async Task<IActionResult> listaEmpleados()
        {
            List<SeleccionDto> lstTipoReferencia = new List<SeleccionDto>();


            lstTipoReferencia = await _IListasCapaNegocios.ListaEmpeadosCompradoresVendedores();

            return Ok(lstTipoReferencia);
        }


        [HttpGet]
        [Authorize]
        [Route("listaUsuariosclienteproveedor")]
        public async Task<IActionResult> listaUsuariosclienteproveedor()
        {
            List<SeleccionDto> lstlistaPaises = new List<SeleccionDto>();

            lstlistaPaises = await _IListasCapaNegocios.ConsultaUsuariosProveedorCliente();

            return Ok(lstlistaPaises);
        }

        [HttpGet]
        [Authorize]
        [Route("listaEstadosForm")]
        public async Task<IActionResult> listaEstadosForm()
        {
            List<SeleccionDto> lstTamañoTercero = new List<SeleccionDto>();

            lstTamañoTercero = await _IListasCapaNegocios.ConsultaEstadosFormularioConstulta();

            return Ok(lstTamañoTercero);
        }


    }
}
