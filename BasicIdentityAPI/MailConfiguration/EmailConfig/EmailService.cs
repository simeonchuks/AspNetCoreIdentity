using BasicIdentityAPI.MailConfiguration.EmailConfig.EmailModels;
using BasicIdentityAPI.MailConfiguration.EmailConfig.Interface;
using BasicIdentityAPI.Models;
using BasicIdentityShared.DataTransferObject;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BasicIdentityAPI.MailConfiguration.EmailConfig
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptionsSnapshot<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }


        //public async Task SendGridSendEmailAsync(string toemail, string subject, string content)
        //{
        //    //var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
        //    var apiKey = _emailSettings.SendGridApiKey;
        //    var client = new SendGridClient(apiKey);
        //    var from = new EmailAddress(_emailSettings.From, "Simeon Chuks");
        //    var to = new EmailAddress(toemail);
        //    var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
        //    var response = await client.SendEmailAsync(msg);
        //}
        public async Task SendEmailAsync(EmailRequest emailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.From);
            email.To.Add(MailboxAddress.Parse(emailRequest.ToEmail));
            email.Subject = emailRequest.Subject;
            email.From.Add(MailboxAddress.Parse(_emailSettings.DisplayName));

            var builder = new BodyBuilder();
            if (emailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in emailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            file.CopyTo(memoryStream);
                            fileBytes = memoryStream.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }

            builder.HtmlBody = emailRequest.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.UserName, _emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            smtp.Dispose();
        }

        public async Task<EmailResponse> SendEmail(EmailRequest email)
        {
            string toEmail = email.ToEmail;
            string subject = email.Subject;
            string body = email.Body;
            
            MailMessage mail = new MailMessage();
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = body;
            mail.From = new MailAddress(_emailSettings.From);
            mail.IsBodyHtml = false;

            //Configure smtp
            System.Net.Mail.SmtpClient smtp = new(_emailSettings.Host);
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential(_emailSettings.UserName, _emailSettings.Password);
            smtp.Send(mail);
            
            return new EmailResponse
            {
                Message = "Email has been to " + mail.To + "Successfully",
                IsSuccess = true,
            };
            throw new NotImplementedException();
        }

        //This method is used to send email directly from the Authentication controller
        public async Task GmailSendEmailAsync(string toemail, string subject, string content)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.From);
            email.To.Add(MailboxAddress.Parse(toemail));
            email.Subject = subject;
            email.From.Add(MailboxAddress.Parse(_emailSettings.DisplayName));

            var builder = new BodyBuilder();
            builder.HtmlBody = content;
            email.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.UserName, _emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        //public async Task<string> SendMail(EmailClass emailClass)
        //{
        //    using (MailMessage mail = new MailMessage())
        //    {
        //        mail.From = new MailAddress(_emailSettings.From);
        //        emailClass.To.ForEach(x =>
        //        {
        //            mail.To.Add(x);
        //        });
        //        mail.Subject = emailClass.Subject;
        //        mail.IsBodyHtml = emailClass.IsBodyHtml;
        //        emailClass.Attachments.ForEach(x =>
        //        {
        //            mail.Attachments.Add(new Attachment(x));
        //        });
        //
        //        using (System.Net.Mail.SmtpClient smtp = new(_emailSettings.Host))
        //        {
        //            smtp.Credentials = new System.Net.NetworkCredential(_emailSettings.UserName, _emailSettings.Password);
        //            smtp.EnableSsl = true;
        //            await smtp.SendMailAsync(mail);
        //            return EmailClassResponse.EmailSent;
        //        }
        //
        //    }
        //}
        //
        //public string GetMailBody(UserRegisterDto userRegister)
        //{
        //    throw new NotImplementedException();
        //}


    }
}
