using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class UsuariolistDto
    {
        public int Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }

        public string Identificacion { get; set; }

        public string Email { get; set; }

        public string Usuario { get; set; }

        public int IdTipoUsuario { get; set; }
  
        public string NombreTipoUsuario { get; set; }

        public bool Activo { get; set; }

        public int IdCompradorVendedor { get; set; }

        public string CorreoCompradorVendedor { get; set; }

    }
}
