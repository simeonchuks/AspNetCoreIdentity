using BasicIdentityAPI.MailConfiguration.SmsConfig.SmsModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Http;

namespace BasicIdentityAPI.MailConfiguration.SmsConfig
{
    public class TwilioClient : ITwilioRestClient
    {
        private readonly TwilioSetting _twilioSetting;
        private readonly ITwilioRestClient _twilioRestClient;
        public TwilioClient(IConfiguration configuration)
        {
            //Customize the underlying HttpClient
            //httpClient.DefaultRequestHeaders.Add();

            _twilioRestClient = new TwilioRestClient(
                configuration["TwilioSms:AccountSid"],
                configuration["TwilioSms:AuthToken"],
                httpClient: new SystemNetHttpClient());
            
            
        }
        public string AccountSid => throw new NotImplementedException();

        public string Region => throw new NotImplementedException();

        public HttpClient HttpClient => throw new NotImplementedException();

        public Response Request(Request request)
        {
            throw new NotImplementedException();
        }

        public Task<Response> RequestAsync(Request request)
        {
            throw new NotImplementedException();
        }
    }
}
