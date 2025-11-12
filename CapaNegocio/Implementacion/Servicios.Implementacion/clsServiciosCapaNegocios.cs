
using CapaDatos.Interfaz.Servicios.Interfaz;
using CapaDatos.util;
using CapaDTO.Peticiones;
using CapaNegocio.Interfaz.Servicios.Interfaz;
using Microsoft.Extensions.Configuration;


namespace CapaNegocio.Implementacion.Servicios.Implementacion
{
    public class clsServiciosCapaNegocios : IServiciosCapaNegocios
    {
        private readonly IServiciosCapaDatos InterfaceServiciosCapaDatos;
        private cDataBase cDataBase;
        private readonly IConfiguration _configuration;
        public clsServiciosCapaNegocios(IConfiguration configuration, IServiciosCapaDatos _interfaceServiciosCapaDatos)
        {
            _configuration = configuration;
            cDataBase = new cDataBase(configuration);
            this.InterfaceServiciosCapaDatos = _interfaceServiciosCapaDatos;
        }


        public async Task<List<ServiciosDto>> ListaServicios(int IdArea)
        { 
        return await InterfaceServiciosCapaDatos.ListaServicios(IdArea);
        }

        public async Task<List<ServiciosDto>> ListaTodosServicios()
        {
            return await InterfaceServiciosCapaDatos.ListaTodosServicios();
        }

        public async Task<bool> CrearServicio(ServiciosDto servicio)
        {
            return await InterfaceServiciosCapaDatos.CrearServicio(servicio);
        }

        public async Task<bool> EditarServicio(ServiciosDto servicio) {
            return await InterfaceServiciosCapaDatos.EditarServicio(servicio);
        }

        public async Task<bool> EliminarServicio(int IdServcio)
        {
            return await InterfaceServiciosCapaDatos.EliminarServicio(IdServcio);
        }


    }
}
