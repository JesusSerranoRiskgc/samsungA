namespace CapaDTO.Peticiones
{
    public class DatosGeneralesDto
    {
        public int Id { get; set; }
        public int IdFormulario { get; set; }
        public string FechaDiligenciamiento { get; set; }
        public int Empresa { get; set; }
        public int TipoSolicitud { get; set; }
        public int ClaseTercero { get; set; }
        public int CategoriaTercero { get; set; }
        public string NombreRazonSocial { get; set; }
        public int TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string? DigitoVerificacion { get; set; }
        public int Pais { get; set; }
        public string Ciudad { get; set; }
        public int TamanoTercero { get; set; }
        public int ActividadEconimoca { get; set; }
        public string DireccionPrincipal { get; set; }
        public string CodigoPostal { get; set; }
        public string CorreoElectronico { get; set; }
        public string Telefono { get; set; }
        public int ObligadoFE { get; set; }
        public string CorreoElectronicoFE { get; set; }
        public int TieneSucursalesOtrosPaises { get; set; }
        public string PaisesOtrasSucursales { get; set; }
        public object PreguntasAdicionales { get; set; }
        public string EstadoCivil { get; set; }
        public string ConyugeIdentificacion { get; set; }
        public bool? tipoPago { get; set; }
        public bool CertBASC { get; set; }
        public bool CertOEA { get; set; }
        public bool CertCTPAT { get; set; }
        public bool CertOtros { get; set; }
        public bool CertNinguno { get; set; }
    }
}
