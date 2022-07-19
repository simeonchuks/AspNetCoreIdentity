using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicIdentityAPI.MailConfiguration.EmailConfig.EmailModels
{
    public class EmailResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
