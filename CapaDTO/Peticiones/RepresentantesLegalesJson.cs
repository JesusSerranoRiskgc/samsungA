using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{

    public class CargosPublicos
    {
        public string NombreEntidad { get; set; }
        public string FechaIngreso { get; set; }
        public string FechaDesvinculacion { get; set; }
    }

    public class VinculosMas
    {
        public string NombreCompleto { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string Pais { get; set; }
        public string PorcentajeParticipacion { get; set; }
    }

    public class InfoFamiliaPep
    {
        public string NombreCompleto { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string Nacionalidad { get; set; }
        public string VinculoFamiliar { get; set; }
    }

    public class Representante
    {
        public string nombre { get; set; }
        public string tipoDocumento { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string Nacionalidad { get; set; }
        public string Telefono { get; set; }
        public string CorreoElectronico { get; set; }
        public string vinculadoPep { get; set; }
        public string ManejaRecursos { get; set; }
        public string CualesRecursos { get; set; }
        public string PoderPolitico { get; set; }
        public string RamaPoderPublico { get; set; }
        public string CargoPublico { get; set; }
        public string CualCargoPublico { get; set; }
        public string ObligacionTributaria { get; set; }
        public string PaisesObligacionTributaria { get; set; }
        public string CuentasFinancierasExt { get; set; }
        public string PaisesCuentasExt { get; set; }
        public string TienePoderCuentaExtranjera { get; set; }
        public string PaisesPoderCuentaExtranjera { get; set; }
        public string hasidoPep2 { get; set; }
        public List<CargosPublicos> cargosPublicos { get; set; }
        public string Tienevinculosmas5 { get; set; }
        public List<VinculosMas> Vinculosmas { get; set; }
        public List<InfoFamiliaPep> InfoFamiliaPep { get; set; }
    }


    public class Directivos
    {
        public string nombre { get; set; }
        public string tipoDocumento { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string Nacionalidad { get; set; }
        public string vinculadoPep { get; set; }
        public string ManejaRecursos { get; set; }
        public string CualesRecursos { get; set; }
        public string PoderPolitico { get; set; }
        public string RamaPoderPublico { get; set; }
        public string CargoPublico { get; set; }
        public string CualCargoPublico { get; set; }
        public string ObligacionTributaria { get; set; }
        public string PaisesObligacionTributaria { get; set; }
        public string CuentasFinancierasExt { get; set; }
        public string PaisesCuentasExt { get; set; }
        public string TienePoderCuentaExtranjera { get; set; }
        public string PaisesPoderCuentaExtranjera { get; set; }
        public string hasidoPep2 { get; set; }
        public List<CargosPublicos> cargosPublicos { get; set; }
        public string Tienevinculosmas5 { get; set; }
        public List<VinculosMas> Vinculosmas { get; set; }
        public List<InfoFamiliaPep> InfoFamiliaPep { get; set; }
    }

    public class RootRepresentante {
        public List<Representante> Representantes { get; set; }
    
    }


    public class RootDirectivo
    {
        public List<Directivos> Directivos { get; set; }

    }


    public class RootAccionistas
    {
        public List<Accionista> Accionista { get; set; }

    }



    public class RootDatosGenerarles
    {
        public string VinculadoPep { get; set; }
        public string ManejaRecursos { get; set; }
        public string CualesRecursos { get; set; }
        public string PoderPolitico { get; set; }
        public string RamaPoderPublico { get; set; }
        public string CargoPublico { get; set; }
        public string CualCargoPublico { get; set; }
        public string ObligacionTributaria { get; set; }
        public List<string> PaisesObligacionTributaria { get; set; }
        public string CuentasFinancierasExt { get; set; }
        public List<string> PaisesCuentasExt { get; set; }
        public string TienePoderCuentaExtranjera { get; set; }
        public List<string> PaisesPoderCuentaExtranjera { get; set; }
        public string HasidoPep2 { get; set; }
        public List<CargosPublicos> cargosPublicos { get; set; }
        public string Tienevinculosmas5 { get; set; }
        public List<VinculosMas> Vinculosmas { get; set; }
        public List<InfoFamiliaPep> InfoFamiliaPep { get; set; }
        public List<BeneficiarioFinal> BeneficiariosFinales { get; set; }

    }




    public class Accionista
    {
        public string NombreCompleto { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string Nacionalidad { get; set; }
        public int Porcentajeparticipacion { get; set; }
        public string VinculadoPep { get; set; }
        public string ManejaRecursos { get; set; }
        public string CualesRecursos { get; set; }
        public string PoderPolitico { get; set; }
        public string RamaPoderPublico { get; set; }
        public string CargoPublico { get; set; }
        public string CualCargoPublico { get; set; }
        public string ObligacionTributaria { get; set; }
        public string PaisesObligacionTributaria { get; set; }
        public string CuentasFinancierasExt { get; set; }
        public string PaisesCuentasExt { get; set; }
        public string TienePoderCuentaExtranjera { get; set; }
        public string PaisesPoderCuentaExtranjera { get; set; }
        public string HasidoPep2 { get; set; }
        public List<CargosPublicos> cargosPublicos { get; set; }
        public string Tienevinculosmas5 { get; set; }
        public List<VinculosMas> Vinculosmas { get; set; }
        public List<InfoFamiliaPep> InfoFamiliaPep { get; set; }
        public List<BeneficiarioFinal> BeneficiariosFinales { get; set; }
    }


    public class BeneficiarioFinal
    {
        public string NombreCompleto { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string Nacionalidad { get; set; }
        public int Porcentajeparticipacion { get; set; }
        public string VinculadoPep { get; set; }
        public string ManejaRecursos { get; set; }
        public string CualesRecursos { get; set; }
        public string PoderPolitico { get; set; }
        public string RamaPoderPublico { get; set; }
        public string CargoPublico { get; set; }
        public string CualCargoPublico { get; set; }
        public string ObligacionTributaria { get; set; }
        public string PaisesObligacionTributaria { get; set; }
        public string CuentasFinancierasExt { get; set; }
        public string PaisesCuentasExt { get; set; }
        public string TienePoderCuentaExtranjera { get; set; }
        public string PaisesPoderCuentaExtranjera { get; set; }
        public string HasidoPep2 { get; set; }
        public List<CargosPublicos> cargosPublicos { get; set; }
        public string Tienevinculosmas5 { get; set; }
        public List<VinculosMas> Vinculosmas { get; set; }
        public List<InfoFamiliaPep> InfoFamiliaPep { get; set; }
    }



    public class Registro
    {
        public int IdFormulario { get; set; }
        public string JsonData { get; set; } // Almacena el JSON como string
                                             //public List<Representante> representantes { get; set; }
    }

    public class RepresentanteLegal
    {
        public string JsonData { get; set; } // Almacena el JSON como string
    }

    public class accionistas
    {
        public string JsonData { get; set; } // Almacena el JSON como string
    }
}

