using CapaDTO.Peticiones;
using CapaDTO.Respuestas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Interfaz.Reporte.Interzas
{
    public interface IReporteCapaNegocios
    {
        Task<List<defaulchartsDto>> ReporteRegistroTiemposxarea(int IdUsuario);
        Task<List<defaulchartsDto>> ReporteRegistroTiemposxUsuario(int IdUsuario);

        Task<List<defaulchartsDto>> ReporteRegistroTiemposxCliente(int IdUsuario);
        Task<List<defaulchartsDto>> ReporteRegistroTiemposxServicio(int IdUsuario);



        Task<MemoryStream> ReporteFormulario(ReporteDto objetofiltro);
    }
}
