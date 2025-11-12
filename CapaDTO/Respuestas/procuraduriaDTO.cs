using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Respuestas
{
    public class RootProcuraduria
    {
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public DataResponse Data { get; set; }
        public object ListData { get; set; } // Si es una lista, cambia a List<T>
    }

    public class DataResponse
    {
        public string HtmlResponse { get; set; }
        public object NotCriminalRecords { get; set; } // Si tiene datos en el futuro, cambia el tipo
        public List<CitizenData> Data { get; set; }
    }

    public class CitizenData
    {
        public string Identification { get; set; }
        public string Name { get; set; }
        public string NumSiri { get; set; }
        public List<Sancion> Sanciones { get; set; }
        public List<Delito> Delitos { get; set; }
        public List<Instancia> Instancias { get; set; }
        public List<Evento> Eventos { get; set; }
        public List<Inhabilidad> Inhabilidades { get; set; }
    }

    public class Sancion
    {
        public string sancion { get; set; }
        public string termino { get; set; }
        public string clase { get; set; }
        public string suspendida { get; set; }
        public object suspension_art { get; set; } // Si es int, cambia a int?
    }

    public class Delito
    {
        public string Descripcion { get; set; }
    }

    public class Instancia
    {
        public string Nombre { get; set; }
        public string Autoridad { get; set; }
        public string fecha_provincia { get; set; }
        public string fecha_efecto_juridicos { get; set; }
    }

    public class Evento
    {
        public string nombre_causa { get; set; }
        public string entidad { get; set; }
        public string tipo_acto { get; set; }
        public string fecha_acto { get; set; }
    }

    public class Inhabilidad
    {
        public string siri { get; set; }
        public string modulo { get; set; }
        public string inhabilidad_legal { get; set; }
        public string fecha_inicio { get; set; }
        public string fecha_fin { get; set; }
        public object suspension_art { get; set; }
    }


}
