using BasicIdentityAPI.MailConfiguration.EmailConfig;
using BasicIdentityAPI.MailConfiguration.EmailConfig.Interface;
using BasicIdentityAPI.Models;
using BasicIdentityAPI.Services.Interface;
using BasicIdentityShared.DataTransferObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicIdentityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly ISendgridService _sengridService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IEmailService emailService, ISendgridService sengridService, IConfiguration configuration)
        {
            _userService = userService;
            _emailService = emailService;
            _sengridService = sengridService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] UserRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUserAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result); //ok returns 200
                }

                return BadRequest(result);
            }

            return BadRequest("ModelState not Valid"); //Means something is wrong from the client side
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync([FromBody] UserLoginDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUserAsync(model);
                if (result.IsSuccess)
                {
                    await _sengridService.SendEmailAsync(model.Email, "New LogIn", "<h1>Hello!, new login to your account</h1><p>New login to your account at " + DateTime.Now + "</p>");
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("ModelState not Valid");
        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("User not Found");
            }

            var result = await _userService.ConfirmEmailAsync(userId, token);

            if (result.IsSuccess)
            {
                return Ok();
                
            }

            return BadRequest(result);
        }

        [HttpGet("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return NotFound();
            }

            var result = await _userService.ForgetPasswordAsync(email);

            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto passwordDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ResetPasswordAsync(passwordDto);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }

            return BadRequest("Some Properties was not valid");
        }

    }
}
