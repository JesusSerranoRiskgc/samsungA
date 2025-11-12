using CapaDTO.Peticiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Interfaz.Area.Interfaz
{
    public interface IAreasCapaNegocios
    {
        Task<List<AreaDto>> ListaAreas();
        Task<bool> CrearArea(AreaDto objarea);
        Task<bool> EditarArea(AreaDto objarea);
        Task<bool> DeleteArea(int IdArea);
    }
}
