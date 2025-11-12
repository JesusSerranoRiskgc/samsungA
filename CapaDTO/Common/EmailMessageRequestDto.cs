using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Common
{
    public class EmailMessageRequestDto
    {
        public string? From { get; set; }
        public List<string> To { get; set; }
        public List<string> ToCC { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public List<string>? AttachmentsRoutes { get; set; }
    }
}
