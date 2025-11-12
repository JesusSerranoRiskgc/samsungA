using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class AdjuntosDto
    {
       
        public IFormFile RUT { get; set; }

      
        public IFormFile CamaraComercio { get; set; }

       
        public IFormFile FotocopiaIdRepresentanteLegal { get; set; }

       
        public IFormFile CertificacionBancaria { get; set; }

       
        public IFormFile CertificacionesOtras { get; set; }

        
        public IFormFile VisitaSeguridad { get; set; }

        
        public IFormFile AcuerdoSeguridad { get; set; }

       
        public IFormFile PlanContingencia { get; set; }

        
        public IFormFile SoporteDebidaDiligencia { get; set; }

       
        public IFormFile AdhesionCodigoConducta { get; set; }

        
        public IFormFile Otros { get; set; }


    }
}
