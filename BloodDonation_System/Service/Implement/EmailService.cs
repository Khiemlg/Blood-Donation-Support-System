using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using BloodDonation_System.Service.Interface;

namespace BloodDonation_System.Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["EmailSettings:SenderEmail"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _config["EmailSettings:SmtpServer"],
                int.Parse(_config["EmailSettings:Port"]),
                SecureSocketOptions.StartTls // ✅ Sử dụng STARTTLS cho port 587
            );

            await smtp.AuthenticateAsync(
                _config["EmailSettings:SenderEmail"],
                _config["EmailSettings:AppPassword"]
            );

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
