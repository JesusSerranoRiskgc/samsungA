using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }

        public string Identificacion { get; set; }

        public string Email { get; set; }

        public int IdTipoUsuario { get; set; }

        public string? Password { get; set; }

        public int idCompradorVendedor { get; set; }

        public bool Activo { get; set; }



    }
}
