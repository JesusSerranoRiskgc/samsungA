using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.ERP
{
    public class ERPDTO
    {
        public string Nit { get; set; }
        public string RazonSocial { get; set; }
        public string Tipo { get; set; }
        public string NombreComercial { get; set; }
        public string DireccionCobro1 { get; set; }
        public string DireccionCobro2 { get; set; }
        public string DireccionDespacho1 { get; set; }
        public string DireccionDespacho2 { get; set; }
        public string Ciudad { get; set; }
        public string Pais { get; set; }
        public string CodPostal { get; set; }
        public string Departamento { get; set; }
        public string OrigenNE { get; set; }
        public string TipoContribuyente { get; set; }
        public string ActEconomica { get; set; }
        public string TipoIdentificacion { get; set; }
        public string Compania { get; set; }
        public string DirCorreo { get; set; }
        public List<Contacto> Contactos { get; set; }
    }



    public class Contacto
    {
        public int IdContacto { get; set; }
        public string NombreContacto { get; set; }
        public string TituloContacto { get; set; }
        public string TipoDocumento { get; set; }
        public string Nit { get; set; }
        public string DirecCorreo { get; set; }
        public string Prefijo1 { get; set; }
        public string TipoTelef1 { get; set; }
        public string NroTelefono1 { get; set; }
        public string Prefijo2 { get; set; }
        public string TipoTelef2 { get; set; }
        public string NroTelefono2 { get; set; }
        public string Ciudad { get; set; }
        public string Direccion { get; set; }
    }


}
