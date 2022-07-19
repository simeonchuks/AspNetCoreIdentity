using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicIdentityAPI.MailConfiguration.SmsConfig.SmsModels
{
    public class TwilioSetting
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
    }
}
