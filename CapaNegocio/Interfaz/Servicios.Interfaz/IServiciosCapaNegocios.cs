using CapaDTO.Peticiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Interfaz.Servicios.Interfaz
{
    public interface IServiciosCapaNegocios
    {
        Task<List<ServiciosDto>> ListaServicios(int IdArea);

        Task<bool> CrearServicio(ServiciosDto servicio);

        Task<bool> EditarServicio(ServiciosDto servicio);

        Task<bool> EliminarServicio(int IdServcio);

        Task<List<ServiciosDto>> ListaTodosServicios();

    }
}
