public class ConflictoInteresDto
{
        public int Id { get; set; }
        public int IdFormulario { get; set; }
        public DateTime? FechaDeclaracion { get; set; }
        public string? CiudadDeclaracion { get; set; }
        public string? NombresApellidos { get; set; }
        public string? Cedula { get; set; }
        public string? TipoVinculacionSamsung { get; set; }
        public bool? ConoceProcedimientoConflicto { get; set; }
        public string? RazonNoConocerProcedimientoConflicto { get; set; }
        public object? EntidadesRelacionSamsung { get; set; }
        public string? OtrasSituacionesConflicto { get; set; }
        public bool? ActividadesCompetidor { get; set; }
        public string? RazonActividadesCompetidor { get; set; }
        public bool? RelacionesEstado { get; set; }
        public string? RazonRelacionesEstado { get; set; }
        public bool? RegalosHospitalidad { get; set; }
        public string? RazonRegalosHospitalidad { get; set; }
        public object? ParentescosTercerGrado { get; set; }
        public bool? OtrasSituacionesAfectanIndependencia { get; set; }
        public string? RazonOtrasSituacionesAfectanIndependencia { get; set; }
        public bool? InfluenciaIndebidaPoliticas { get; set; }
        public bool? InfluenciaIndebidaAdjudicaciones { get; set; }
        public bool? DescuentoReventa { get; set; }
        public bool? ActividadesRegulatorias { get; set; }
        public bool? CorredorIntermediario { get; set; }
        public bool? RegalosFuncionarios { get; set; }
        public bool? ApruebaTransaccionesConflicto { get; set; }
    
}