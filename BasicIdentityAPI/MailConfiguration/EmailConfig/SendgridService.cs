using BasicIdentityAPI.MailConfiguration.EmailConfig.EmailModels;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicIdentityAPI.MailConfiguration.EmailConfig
{

    public interface ISendgridService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);

        Task Execute(string apiKey, string subject, string message, string toEmail);

        Task<EmailResponse> SendEmail(EmailRequest email);
    }

    public class SendgridService : ISendgridService
    {
        private readonly SendgridConfig _sendgridOption;

        public SendgridService(IOptionsSnapshot<SendgridConfig> sendgridOption)
        {
            _sendgridOption = sendgridOption.Value;
        }

        public async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_sendgridOption.SenderEmail, _sendgridOption.SendGridUser),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));

            msg.SetClickTracking(false, false);
            await client.SendEmailAsync(msg);

            return;
        }

        public Task SendEmailAsync(string toEmail, string subject, string message)
        {
            return Execute(_sendgridOption.SendGridApiKey, subject, message, toEmail);
        }

        public async Task<EmailResponse> SendEmail(EmailRequest email)
        {
            var client = new SendGridClient(_sendgridOption.SendGridApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_sendgridOption.SenderEmail, _sendgridOption.SendGridUser),
                Subject = email.Subject,
                PlainTextContent = email.Body,
                HtmlContent = email.Body
            };
            msg.AddTo(new EmailAddress(email.ToEmail));

            msg.SetClickTracking(false, false);
            await client.SendEmailAsync(msg);

            return new EmailResponse
            {
                Message = "Email has been to " + email.ToEmail + "Successfully",
                IsSuccess = true,
            };
        }


    }
}
