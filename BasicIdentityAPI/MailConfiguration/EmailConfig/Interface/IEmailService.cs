using BasicIdentityAPI.MailConfiguration.EmailConfig.EmailModels;
using BasicIdentityShared.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicIdentityAPI.MailConfiguration.EmailConfig.Interface
{
    public interface IEmailService
    {
        Task<EmailResponse> SendEmail(EmailRequest email);
        Task SendEmailAsync(EmailRequest emailRequest);

        //Task<string> SendMail(EmailClass emailClass);
        //string GetMailBody(UserRegisterDto userRegister);

        //Task SendGridSendEmailAsync(string toemail, string subject, string content);

        Task GmailSendEmailAsync(string toemail, string subject, string content);
    }
}
