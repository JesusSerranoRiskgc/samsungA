using CapaDTO.Peticiones;
using CapaNegocio.Interfaz.auth.interfaz;
using CapaNegocio.Interfaz.RegistroFormulario.Interface;
using CapaNegocio.Interfaz.Reporte.Interzas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EnkaAPI.Controllers.ReporteForm
{

    [Route("api/[controller]")]
    [ApiController]
    public class ReporteFormController : ControllerBase
    {


        private readonly IWebHostEnvironment _env;

        private readonly IHttpClientFactory _clientFactory;

        private readonly IConfiguration _configuration;

        protected readonly IloginCapaNegocios _iloginCapaNegocios;
        protected readonly IRegistroFormularioCapaNegocio _RegistoFomulario;

        protected readonly IReporteCapaNegocios _ReporteControles;


        public ReporteFormController(IWebHostEnvironment env, IloginCapaNegocios iloginCapaNegocios, IConfiguration _configuration, IRegistroFormularioCapaNegocio RegistoFomulario, IHttpClientFactory clientFactory, IReporteCapaNegocios iReporteCapaNegocios)
        {
            _clientFactory = clientFactory;
            this._configuration = _configuration;
            this._iloginCapaNegocios = iloginCapaNegocios;
            this._RegistoFomulario = RegistoFomulario;
            _env = env;
            this._ReporteControles = iReporteCapaNegocios;
        }




        [HttpPost]
        [Route("GenerateExcel")]
        public async Task<IActionResult> GenerateExcel(ReporteDto objetofiltro)
        {
            // Obtén los datos desde la capa de datos o negocios


            // Genera el archivo Exce
            MemoryStream excelStream = await _ReporteControles.ReporteFormulario(objetofiltro);

            // Devuelve el archivo Excel al cliente
            return File(excelStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteFormulario.xlsx");
        }



    }
}
