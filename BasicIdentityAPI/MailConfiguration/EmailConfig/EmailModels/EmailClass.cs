using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicIdentityAPI.MailConfiguration.EmailConfig.EmailModels
{
    public class EmailClass
    {
        public string From { get; set; }
        public List<string> To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; }
        public string Password { get; set; }
        public List<string> Attachments { get; set; }
    }
}
