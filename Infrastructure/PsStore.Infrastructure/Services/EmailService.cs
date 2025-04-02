using PsStore.Application.Interfaces.Services;
using System.Net;
using System.Net.Mail;

namespace PsStore.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com"; // Your SMTP server
        private readonly int _smtpPort = 587;
        private readonly string _smtpUsername = "tahiraa@code.edu.az";
        private readonly string _smtpPassword = "cvvj dgpm ojjf qglb";

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpUsername),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(to);

                using var smtpClient = new SmtpClient(_smtpServer, _smtpPort)
                {
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    EnableSsl = true,
                };

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
