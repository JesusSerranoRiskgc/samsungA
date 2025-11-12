using CapaDTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Interfaz.Email.Interfaz
{
    public interface IEMailService
    {
        Task<string> SendMail(EmailMessageRequestDto emailMessage);
        Task<string> SendMail2(EmailMessageRequestDto emailMessage);
        Task<string> SendMail3(EmailMessageRequestDto emailMessage);

    }
}
