using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class InformacionInspektorDto
    {
        public int Id  { get; set; }
        public int IdFomulario { get; set; }
        public string Tipo_Tercero { get; set; }
        public int Tipo_Identificacion { get; set; }
        public string TipoIdentificacion { get; set; }
        public string Numero_Identificacion { get; set; }
        public string Nombre { get; set; }

        public string Numero_Consulta { get; set; }
        public string Coincidencias { get; set; }

        public string Fecha_Consulta { get; set; }





    }


    public class RespuestaConsulta
    {
        public int NumConsulta { get; set; }
        public int CantCoincidencias { get; set; }
        public string Nombre { get; set; }
        public string NumDocumento { get; set; }
        public List<Lista> Listas { get; set; }
    }

    public class Lista
    {
        public int IdLista { get; set; }
        public DateTime Fecha { get; set; }
        public string Prioridad { get; set; }
        public string NombreTipoLista { get; set; }
        public string NombreGrupoLista { get; set; }
        public int IdGrupoLista { get; set; }
        public string TipoDocumento { get; set; }
        public string DocumentoIdentidad { get; set; }
        public string NombreCompleto { get; set; }
        public int IdTipoLista { get; set; }
        public string FuenteConsulta { get; set; }
        public string TipoPersona { get; set; }
        public string Alias { get; set; }
        public string Delito { get; set; }
    }

}
