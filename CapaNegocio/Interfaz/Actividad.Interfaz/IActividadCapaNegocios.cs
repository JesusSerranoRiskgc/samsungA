using CapaDTO.Peticiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Interfaz.Actividad.Interfaz
{
    public interface IActividadCapaNegocios
    {
        Task<List<ActividadDto>> ListaActividad();
        Task<List<ActividadDto>> ListaActividadbyIdArea(int idArea);

        Task<bool> CrearActividad(ActividadDto objActividad);

        Task<bool> EditarActividad(ActividadDto objActividad);

        Task<bool> DeleteActividad(int IdActividad);
    }
}
