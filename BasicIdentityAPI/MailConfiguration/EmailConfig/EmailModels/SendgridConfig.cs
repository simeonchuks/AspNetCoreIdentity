using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicIdentityAPI.MailConfiguration.EmailConfig.EmailModels
{
    public class SendgridConfig
    {
        public string SendGridUser { get; set; }
        public string SendGridApiKey { get; set; }
        public string SenderEmail { get; set; }
        public string AppUrl { get; set; }
    }
}
