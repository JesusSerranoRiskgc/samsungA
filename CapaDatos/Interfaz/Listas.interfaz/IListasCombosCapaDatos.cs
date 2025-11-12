using CapaDTO.Respuestas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos.Interfaz.Listas.interfaz
{
    public interface IListasCombosCapaDatos
    {
        Task<List<SeleccionDto>> ListaSiNo(string Lang);

        Task<List<SeleccionDto>> ConsultaTipoSolicitud(string Lang);

        Task<List<SeleccionDto>> ClaseTercero(string Lang);

        Task<List<SeleccionDto>> CategoriaTercero(string Lang);
        Task<List<SeleccionDto>> Paises(string Lang);
        Task<List<SeleccionDto>> TamañoTercero(string Lang);
        Task<List<SeleccionDto>> ActividadEconomica(string Lang);

        Task<List<SeleccionDto>> TiposDocumentos(string Lang);

        Task<List<SeleccionDto>> listaTipoCuentaBancaria(string Lang);

        Task<List<SeleccionDto>> listaTipoReferenciaBanCom(string Lang);

        Task<List<SeleccionDto>> ListaEmpeadosCompradoresVendedores();

        Task<List<SeleccionDto>> TipoUsuario();

        Task<List<SeleccionDto>> ConsultaEstadosFormularioConstulta();

        Task<List<SeleccionDto>> ConsultaUsuariosProveedorCliente();
    }
}
