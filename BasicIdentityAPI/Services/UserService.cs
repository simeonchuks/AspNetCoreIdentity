using BasicIdentityAPI.MailConfiguration.EmailConfig;
using BasicIdentityAPI.Models;
using BasicIdentityAPI.Services.Interface;
using BasicIdentityShared;
using BasicIdentityShared.DataTransferObject;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BasicIdentityAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ISendgridService _sendgridService;

        public UserService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, ISendgridService sendgridService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _sendgridService = sendgridService;
        }

        public async Task<UserRegResponse> RegisterUserAsync(UserRegisterDto model)
        {
            UserRegResponse regResponse = new();
            
            if (model == null)
                throw new NullReferenceException("UserRegisterDto is null");

            //Check if password match
            if (model.Password != model.ComfirmPassword)
            {
                //return new UserManagerResponse
                //{
                //    Message = "Password do not match",
                //    IsSuccess = false
                //};
                regResponse.Message = "Password do not match";
                regResponse.IsSuccess = false;
            }

            var IdentityUser = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(IdentityUser, model.Password);
            if (result.Succeeded)
            {
                //Send Confirmation Email
                // Generate a token
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(IdentityUser);
                //Encode the token to a byte aray
                var encodeEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                //Generate a valid string that contains non special character
                var validEmailToken = WebEncoders.Base64UrlEncode(encodeEmailToken);
                //Generate the url
                string url = $"{_configuration["SendgridConfig:AppUrl"]}/api/auth/confirmemail?userid={IdentityUser.Id}&token={validEmailToken}";
                //send the valid token to the user via the url
                await _sendgridService.SendEmailAsync(IdentityUser.Email, "Confirm Email", $"<h1>WELCOME</h1> " + $" <p>Click on the link <a href='{url}'>Click Here</a>to confirm your email</p>");

                regResponse.IsSuccess = true;
                regResponse.Message = "User Created Successfuly";
                regResponse.Token = validEmailToken;
            }
            else
            {
                regResponse.Message = "An Error Occured, User not Created";
                regResponse.IsSuccess = false;
                regResponse.Errors = result.Errors.Select(e => e.Description);
            }

            return regResponse;
         
        }

        public async Task<UserManagerResponse> LoginUserAsync(UserLoginDto model)
        {
            //get the user and check if the user exist.
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "Email Address not found",
                    IsSuccess = false
                };
            }

            //check the user password if its correct
            var checkPassword = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!checkPassword)
            {
                return new UserManagerResponse
                {
                    Message = "Invalid Email or Password",
                    IsSuccess = false
                };
            }

            //Create array of claims. Note you can use pre define type or you can set your own
            var authClaims = new[]
            {
                new Claim("Email", model.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            //Generate Access Token
            var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256Signature)
                );

            var tokenAsString =  new JwtSecurityTokenHandler().WriteToken(token);

            return new UserManagerResponse
            {
                Message = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };

        }

        public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "User not Found",
                    IsSuccess = false
                };
            }

            // Decode the token, first you have to get the part from the string
            var decodedToken = WebEncoders.Base64UrlDecode(token);
            //Get Token as String
            var tokenAsString = Encoding.UTF8.GetString(decodedToken);
            // Confirm the email
            var result = await _userManager.ConfirmEmailAsync(user, tokenAsString);

            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "Email Confirm Successfully, Kindly login",
                    IsSuccess = true,
                };
            }
            return new UserManagerResponse
            {
                Message = "Email Confirmation failed",
                IsSuccess = false,
                Errors = result.Errors.Select(e=>e.Description)
            };
        }

        public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "Email not found",
                    IsSuccess = false
                };
            }

            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedPasswordToken = Encoding.UTF8.GetBytes(passwordResetToken);
            
            var validPasswordToken = WebEncoders.Base64UrlEncode(encodedPasswordToken);

            string url = $"{_configuration["SendgridConfig:AppUrl"]}/ResetPassword?email={email}&token={validPasswordToken}";

            await _sendgridService.SendEmailAsync(email, "Reset Password", "<h1>Follow the Instruction to reset your password</h1>" + $"<p><a href='{url}'>Click Here</a> to reset your password</p>");

            return new UserManagerResponse
            {
                Message = "Password Reset Successful",
                IsSuccess = true
            };

        }

        public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "Email not found",
                    IsSuccess = false
                };
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return new UserManagerResponse
                {
                    Message = "Password Do not Match",
                    IsSuccess = false
                };
            }

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            var tokenAsString = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ResetPasswordAsync(user, tokenAsString, model.NewPassword);

            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "Password has been reset successfully",
                    IsSuccess = true
                };
            }

            return new UserManagerResponse
            {
                Message = "Something went wrong",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }
    } 
}
