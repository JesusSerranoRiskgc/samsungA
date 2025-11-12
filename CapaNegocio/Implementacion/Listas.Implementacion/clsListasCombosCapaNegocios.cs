using CapaDatos.Interfaz.auth.interfaz;
using CapaDatos.Interfaz.Listas.interfaz;
using CapaDTO.Peticiones;
using CapaDTO.Respuestas;
using CapaNegocio.Interfaz.Email.Interfaz;
using CapaNegocio.Interfaz.Listas.Interfaz;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Implementacion.Listas.Implementacion
{
    public class clsListasCombosCapaNegocios : IListasCapaNegocios
    {
        private readonly IConfiguration _configuration;

        private readonly IListasCombosCapaDatos _ListasCombos;


        public clsListasCombosCapaNegocios(IConfiguration configuration, IListasCombosCapaDatos ListasCombos)
        {
            this._ListasCombos = ListasCombos;
            _configuration = configuration;

        }

        public async Task<List<SeleccionDto>> ListaSiNo(string Lang)
        {
            return await _ListasCombos.ListaSiNo(Lang);
        }

        public async Task<List<SeleccionDto>> ConsultaTipoSolicitud(string Lang)
        {
            return await _ListasCombos.ConsultaTipoSolicitud(Lang);
        }


        public async Task<List<SeleccionDto>> ClaseTercero(string Lang) {
            return await _ListasCombos.ClaseTercero(Lang);
        }
        public async Task<List<SeleccionDto>> CategoriaTercero(string Lang) {
            return await _ListasCombos.CategoriaTercero(Lang);
        }
        public async Task<List<SeleccionDto>> Paises(string Lang) {
            return await _ListasCombos.Paises(Lang);
        }
        public async Task<List<SeleccionDto>> TamañoTercero(string Lang) {
            return await _ListasCombos.TamañoTercero(Lang);
        }
        public async Task<List<SeleccionDto>> ActividadEconomica(string Lang) {
            return await _ListasCombos.ActividadEconomica(Lang);
        }

        public async Task<List<SeleccionDto>> TiposDocumentos(string Lang) {
            return await _ListasCombos.TiposDocumentos(Lang);
        }


        public async Task<List<SeleccionDto>> listaTipoCuentaBancaria(string Lang)
        {
            return await _ListasCombos.listaTipoCuentaBancaria(Lang);
        }

        public async Task<List<SeleccionDto>> listaTipoReferenciaBanCom(string Lang)
        {
            return await _ListasCombos.listaTipoReferenciaBanCom(Lang);
        }

        public async Task<List<SeleccionDto>> ListaEmpeadosCompradoresVendedores()
        {
            return await _ListasCombos.ListaEmpeadosCompradoresVendedores();
        }



        public async Task<List<SeleccionDto>> TipoUsuario()
        {
            return await _ListasCombos.TipoUsuario();
        }

        public async Task<List<SeleccionDto>> ConsultaEstadosFormularioConstulta()
        {
            return await _ListasCombos.ConsultaEstadosFormularioConstulta();
        }



        public async Task<List<SeleccionDto>> ConsultaUsuariosProveedorCliente()
        {
            return await _ListasCombos.ConsultaUsuariosProveedorCliente();
        }


    }
}
