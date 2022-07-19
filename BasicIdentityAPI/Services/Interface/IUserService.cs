using BasicIdentityAPI.Models;
using BasicIdentityShared;
using BasicIdentityShared.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicIdentityAPI.Services.Interface
{
    public interface IUserService
    {
        Task<UserRegResponse> RegisterUserAsync(UserRegisterDto model);

        Task<UserManagerResponse> LoginUserAsync(UserLoginDto model);

        Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);

        Task<UserManagerResponse> ForgetPasswordAsync(string email);

        Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordDto model);
    }
}
