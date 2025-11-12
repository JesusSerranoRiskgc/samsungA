using CapaDTO.Common;
using CapaNegocio.Interfaz.Email.Interfaz;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using SmtpClient2 = System.Net.Mail.SmtpClient;

namespace CapaNegocio.Implementacion.Email.Implementacion
{
    public class EMailService : IEMailService
    {
        private readonly MailboxAddress mailboxAddress;
        private readonly SmtpClient client;
        private readonly string host, userName, password, mailAddress, emailName;
        private readonly int port;
        private readonly SecureSocketOptions encryptionType;
        private readonly IConfiguration _configuration;

        public EMailService(IConfiguration configuration)
        {
            _configuration = configuration;

            //var EmailConfig = configuration.GetSection("EmailConfig");
            host = _configuration["EmailConfig:Host"].ToString();
            port = Convert.ToInt32(_configuration["EmailConfig:Port"]);
            userName = _configuration["EmailConfig:UserName"].ToString();
            password = _configuration["EmailConfig:Password"].ToString();

        }

        public async Task<string> SendMail(EmailMessageRequestDto emailMessage)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Sistema Registro Formularios Cliente/Proveedor", this.userName));
            emailMessage.To.ForEach(y => message.To.Add(MailboxAddress.Parse(y)));
            if (emailMessage.ToCC != null)
            {
                emailMessage.ToCC.ForEach(y => message.Cc.Add(MailboxAddress.Parse(y)));
            }

            message.Subject = emailMessage.Subject;
            var body = new TextPart("plain")
            {
                Text = emailMessage.Body
            };

            var attachments = new List<MimePart>();
            var builder = new BodyBuilder();

            // Set the plain-text version of the message text
            builder.HtmlBody = /*"<div> Descripcion del ticket </div><br><br>" + */ emailMessage.Body;

            var multipart = new Multipart("mixed");
            multipart.Add(builder.ToMessageBody());
            if (emailMessage.AttachmentsRoutes != null)
            {
                emailMessage.AttachmentsRoutes.ForEach(x => attachments.Add(new MimePart()
                {
                    Content = new MimeContent(File.OpenRead(x)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(x)
                }));
                attachments.ForEach(x => multipart.Add(x));
            }

            message.Body = multipart;
            // message.Body = builder.ToMessageBody();

            //  message.Attachments.Add(new Attachment ("fdsfssfd"));

            //message.Body = multipart;
            try
            {
                //connects to the gmail smtp server using port 465 with SSL enabled
                /*   client.Connect(host, port, encryptionType);
                   //needed if the SMTP server requires authentication, like gmail for example                

                   client.Authenticate(userName, password);

                   return client.Send(message);*/

                using (var client = new SmtpClient())
                {
                    client.Connect(this.host, this.port, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(this.userName, this.password);

                    client.Send(message);
                    client.Disconnect(true);
                    client.Dispose();
                }

                foreach (var part in message.BodyParts.OfType<MimePart>())
                    part.Content?.Stream.Dispose();

                return "";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }


            /*  var message = new MimeMessage();
              message.From.Add(new MailboxAddress("juan olaya", "sherlock@riskconsultingcolombia.com"));
              message.To.Add(new MailboxAddress("Mrs. juanes", "ingenieros2@riskgc.com"));
              message.Subject = "envio de prueba ";

              var builder = new BodyBuilder();
              builder.HtmlBody = @"Hey Chandler,

  I just wanted to let you know that Monica and I were going to go play some paintball, you in?

  -- Joey";

              builder.Attachments.Add(@"C:\Users\Administrador\Downloads\consultarEvidenciaPDF (1).pdf");


              message.Body = builder.ToMessageBody();
              try {

                  using (var client = new SmtpClient())
                  {
                      client.Connect(this.host, this.port, false);

                      // Note: only needed if the SMTP server requires authentication
                      client.Authenticate(this.userName, this.password);

                      client.Send(message);
                      client.Disconnect(true);
                  }

                  return "";


              } catch (Exception ex)
              {
                  return ex.Message;
              }
            */

        }

        public async Task<string> SendMail2(EmailMessageRequestDto emailMessage)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Sistema de tickets", this.userName));
            emailMessage.To.ForEach(y => message.To.Add(MailboxAddress.Parse(y)));
            if (emailMessage.ToCC != null)
            {
                emailMessage.ToCC.ForEach(y => message.Cc.Add(MailboxAddress.Parse(y)));
            }

            message.Subject = emailMessage.Subject;
            var body = new TextPart("plain")
            {
                Text = emailMessage.Body
            };

            var attachments = new List<MimePart>();



            var builder = new BodyBuilder();

            // Set the plain-text version of the message text
            builder.HtmlBody = "<div> Gestion del Ticket: </div><br><br>" + emailMessage.Body;

            var multipart = new Multipart("mixed");
            multipart.Add(builder.ToMessageBody());
            if (emailMessage.AttachmentsRoutes != null)
            {
                emailMessage.AttachmentsRoutes.ForEach(x => attachments.Add(new MimePart()
                {
                    Content = new MimeContent(File.OpenRead(x)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(x)
                }));
                attachments.ForEach(x => multipart.Add(x));

            }

            message.Body = multipart;
            // message.Body = builder.ToMessageBody();

            //  message.Attachments.Add(new Attachment ("fdsfssfd"));

            //message.Body = multipart;
            try
            {
                //connects to the gmail smtp server using port 465 with SSL enabled
                /*   client.Connect(host, port, encryptionType);
                   //needed if the SMTP server requires authentication, like gmail for example                

                   client.Authenticate(userName, password);

                   return client.Send(message);*/

                using (var client = new SmtpClient())
                {
                    client.Connect(this.host, this.port, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(this.userName, this.password);

                    client.Send(message);
                    client.Disconnect(true);
                    client.Dispose();
                }

                foreach (var part in message.BodyParts.OfType<MimePart>())
                    part.Content?.Stream.Dispose();

                return "";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }


            /*  var message = new MimeMessage();
              message.From.Add(new MailboxAddress("juan olaya", "sherlock@riskconsultingcolombia.com"));
              message.To.Add(new MailboxAddress("Mrs. juanes", "ingenieros2@riskgc.com"));
              message.Subject = "envio de prueba ";

              var builder = new BodyBuilder();
              builder.HtmlBody = @"Hey Chandler,

  I just wanted to let you know that Monica and I were going to go play some paintball, you in?

  -- Joey";

              builder.Attachments.Add(@"C:\Users\Administrador\Downloads\consultarEvidenciaPDF (1).pdf");


              message.Body = builder.ToMessageBody();
              try {

                  using (var client = new SmtpClient())
                  {
                      client.Connect(this.host, this.port, false);

                      // Note: only needed if the SMTP server requires authentication
                      client.Authenticate(this.userName, this.password);

                      client.Send(message);
                      client.Disconnect(true);
                  }

                  return "";


              } catch (Exception ex)
              {
                  return ex.Message;
              }
            */

        }


        public async Task<string> SendMail3(EmailMessageRequestDto emailMessage)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Sistema de gestion de mantenimiento de equipos", this.userName));
            emailMessage.To.ForEach(y => message.To.Add(MailboxAddress.Parse(y)));
            if (emailMessage.ToCC != null)
            {
                emailMessage.ToCC.ForEach(y => message.Cc.Add(MailboxAddress.Parse(y)));
            }

            message.Subject = emailMessage.Subject;
            var body = new TextPart("plain")
            {
                Text = emailMessage.Body
            };

            var attachments = new List<MimePart>();



            var builder = new BodyBuilder();

            // Set the plain-text version of the message text
            builder.HtmlBody = "<div> Descripcion del Mantenimiento </div><br><br>" + emailMessage.Body;

            var multipart = new Multipart("mixed");
            multipart.Add(builder.ToMessageBody());
            if (emailMessage.AttachmentsRoutes != null)
            {
                emailMessage.AttachmentsRoutes.ForEach(x => attachments.Add(new MimePart()
                {
                    Content = new MimeContent(File.OpenRead(x)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(x)
                }));
                attachments.ForEach(x => multipart.Add(x));





            }

            message.Body = multipart;

            try
            {


                using (var client = new SmtpClient())
                {
                    client.Connect(this.host, this.port, false);

                    client.Authenticate(this.userName, this.password);

                    client.Send(message);
                    client.Disconnect(true);
                    client.Dispose();
                }

                foreach (var part in message.BodyParts.OfType<MimePart>())
                    part.Content?.Stream.Dispose();

                return "";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }


            /*  var message = new MimeMessage();
              message.From.Add(new MailboxAddress("juan olaya", "sherlock@riskconsultingcolombia.com"));
              message.To.Add(new MailboxAddress("Mrs. juanes", "ingenieros2@riskgc.com"));
              message.Subject = "envio de prueba ";

              var builder = new BodyBuilder();
              builder.HtmlBody = @"Hey Chandler,

  I just wanted to let you know that Monica and I were going to go play some paintball, you in?

  -- Joey";

              builder.Attachments.Add(@"C:\Users\Administrador\Downloads\consultarEvidenciaPDF (1).pdf");


              message.Body = builder.ToMessageBody();
              try {

                  using (var client = new SmtpClient())
                  {
                      client.Connect(this.host, this.port, false);

                      // Note: only needed if the SMTP server requires authentication
                      client.Authenticate(this.userName, this.password);

                      client.Send(message);
                      client.Disconnect(true);
                  }

                  return "";


              } catch (Exception ex)
              {
                  return ex.Message;
              }
            */

        }



        public static void EnviarCorreo(string destinario, string cuerpo, string asunto)
        {

            try
            {
                MailMessage objMessage = new MailMessage();
                SmtpClient2 objSmtpClient = new SmtpClient2();
                MailAddress maFromAddress = new MailAddress(((System.Net.NetworkCredential)(objSmtpClient.Credentials)).UserName, "Software Sherlock");
                objMessage.IsBodyHtml = true;
                objMessage.From = maFromAddress;
                objMessage.Subject = asunto;
                objMessage.Body = $"Buen día, <br /><br />{cuerpo}<br /><br /><br />***Nota: Este correo es enviado autómaticamente por Sherlock.";
                objMessage.To.Add(destinario);
                objSmtpClient.Send(objMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
