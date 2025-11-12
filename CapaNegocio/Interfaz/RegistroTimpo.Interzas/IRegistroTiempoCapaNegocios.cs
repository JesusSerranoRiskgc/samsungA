using CapaDTO.Peticiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Interfaz.RegistroTimpo.Interzas
{
    public interface IRegistroTiempoCapaNegocios
    {
        Task<bool> CrearRegistroTiempo(RegistroTiempoDto objRegistro);

        Task<List<RegistroTiempoDto>> ListaRegistroTiempos(int Idusuario);
        Task<List<RegistroTiempoDto>> ListaRegistroTiemposByUser(int IdUsuario);
        Task<bool> EditaRegistroTiempo(RegistroTiempoDto objRegistro);

        Task<bool> EliminarRegistroTiempo(int IdRegistro);
    }
}
