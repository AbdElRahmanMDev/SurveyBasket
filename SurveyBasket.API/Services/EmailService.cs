using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using SurveyBasket.API.Authentication;

namespace SurveyBasket.API.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly MailOptions _mailSettings;
        private readonly ILogger<EmailSender> _logger;
        public EmailSender(IOptions<MailOptions> sender, ILogger<EmailSender> logger)
        {
            _mailSettings = sender.Value;
            _logger = logger;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.Mail),
                Subject = subject
            };

            message.To.Add(MailboxAddress.Parse(email));

            var builder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };

            message.Body = builder.ToMessageBody();




            using var smtp = new SmtpClient();

            _logger.LogInformation("Sending email to {email}", email);

            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(message);
            smtp.Disconnect(true);

        }



    }
}
