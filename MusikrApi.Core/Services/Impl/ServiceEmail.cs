using MusikrApi.Core.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MusikrApi.Core.Services.Impl
{
    public class ServiceEmail : IServiceEmail
    {
        public void EnviaEmail(string email, string assunto, string body)
        {

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("api.musikr@gmail.com", "a1S@d3F$g5")
            };
            using (var message = new MailMessage("api.musikr@gmail.com", email)
            {
                Subject = assunto,
                Body = body, 
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}
