using CapaDTO.Peticiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos.Interfaz.Servicios.Interfaz
{
    public interface IServiciosCapaDatos
    {
        Task<List<ServiciosDto>> ListaServicios(int IdArea);

        Task<bool> CrearServicio(ServiciosDto servicio);

        Task<bool> EditarServicio(ServiciosDto servicio);

        Task<bool> EliminarServicio(int IdServcio);

        Task<List<ServiciosDto>> ListaTodosServicios();
    }
}
