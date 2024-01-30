using AspNetCoreIdentityApp.Core.OptionsModels;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace AspNetCoreIdentityApp.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> config)
        {
            _settings = config.Value;
        }

        public async Task SendResetPasswordEmail(string resetEmailLink, string toUserEmail)
        {
            var smtp = new SmtpClient();
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Port = 587;
            smtp.Host=_settings.Host;
            smtp.Credentials = new NetworkCredential(_settings.Email, _settings.Password);
            smtp.EnableSsl = true;
            var mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(_settings.Email);
            mailMessage.To.Add(toUserEmail);
            mailMessage.Subject = "Şifre Sıfırlama Linki";
            mailMessage.Body = $"<h4>Şifrenizi yenilemek için aşağıdaki bağlantıya tıklayınız</h4> \n <p> <a href='{resetEmailLink}'>{resetEmailLink}</a></p>";

            mailMessage.IsBodyHtml = true;

            await smtp.SendMailAsync(mailMessage);
        }
    }
}
