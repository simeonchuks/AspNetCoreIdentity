using BasicIdentityAPI.MailConfiguration.EmailConfig;
using BasicIdentityAPI.MailConfiguration.EmailConfig.EmailModels;
using BasicIdentityAPI.MailConfiguration.EmailConfig.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicIdentityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ISendgridService _sendgridService;

        public EmailController(IEmailService emailService, ISendgridService sendgridService)
        {
            _emailService = emailService;
            _sendgridService = sendgridService;
        }

        [HttpPost("sendemailasync")]
        public async Task<IActionResult> SendEmailAsync([FromBody] EmailRequest emailRequest)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _emailService.SendEmailAsync(emailRequest);
                    return Ok("Email Sent Successfully");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return BadRequest("Email not sent");
        }

        [HttpPost("sendemail")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
        {
            if (ModelState.IsValid)
            {
                var result = await _emailService.SendEmail(emailRequest);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("ModelState not Valid");
        }

        [HttpPost("sendemail1")]
        public async Task<IActionResult> SendgridSendEmail([FromBody] EmailRequest emailRequest)
        {
            if (ModelState.IsValid)
            {
                var result = await _sendgridService.SendEmail(emailRequest);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("ModelState not Valid");
        }

    }
}
