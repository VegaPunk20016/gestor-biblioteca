using Microsoft.Extensions.Configuration;
using ReadHub.Application.Interfaces;
using System.Net;
using System.Net.Mail;

namespace ReadHub.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpServer = _config["EmailSettings:SmtpServer"];
            var port = int.Parse(_config["EmailSettings:Port"]);
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderName = _config["EmailSettings:SenderName"];
            var password = _config["EmailSettings:Password"];

            var message = new MailMessage
            {
                From = new MailAddress(senderEmail!, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            using var client = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(senderEmail, password),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }
    }
}
