using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Respuestas
{
    public class EjecucionPenas
    {
        public string queryParam { get; set; }
        public string nameResult { get; set; }
        public string identificationNumberResult { get; set; }
        public string cityName { get; set; }
        public string link { get; set; }
        public string license { get; set; }
        public DateTime queryDate { get; set; }
        public bool isSuccess { get; set; }
    }

    public class DataDto
    {
        public string QueryParam { get; set; }
        public string NameResult { get; set; }
        public string IdentificationNumberResult { get; set; }
        public string CityName { get; set; }
        public string Link { get; set; }
        public string License { get; set; }
        public DateTime QueryDate { get; set; }
        public bool IsSuccess { get; set; }
    }
}
