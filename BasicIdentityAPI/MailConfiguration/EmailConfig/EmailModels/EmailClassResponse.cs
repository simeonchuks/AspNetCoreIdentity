using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicIdentityAPI.MailConfiguration.EmailConfig.EmailModels
{
    public static class EmailClassResponse
    {
        public static string Success = "SUCCESS"; 
        public static string ErrorFound = "ErrorFound"; 
        public static string UserAlreadyExist = "User Already Exist"; 
        public static string VerifyEmail = "Very Your Email"; 
        public static string InvaldUser = "Invalid User, Create an Account"; 
        public static string EmailSent = "Email Sent"; 
        public static string UserCreated = "User Created, Verify your Email"; 
    }
}
