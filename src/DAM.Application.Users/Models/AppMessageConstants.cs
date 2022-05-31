using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Users.Models
{
    public class AppMessageConstants
    {
        public const string RegistrationFailed = "Registration failed.";
        public const string LoginFailed = "Login failed.";
        public const string InvalidEmail = "Email address not found.";
        public const string InvalidPassword = "Invalid Password.";
        public const string EmailUnconfirmed = "Please confirm email address.";
        public const string RegistrationMessage = "You have been added as a new user of DAM. Please confirm email to Log in.";
        public const string AddPasswordFailed = "Create new password failed.";
        public const string ResetPasswordFailed = "Reset password failed.";
        public const string HasPassword = "User already has a password.";
        public const string EmailConfirmationFailed = "Email confirmation failed.";
        public const string InvalidInputs = "Inputs invalid.";
        public const string DeactivatedAccount = "Your account has been deactivated. Please contact your administrator.";
        public const string EmailAlreadyInUse = "Email is already in use.";
        public const string EmailAlreadyConfirmed = "Email is already confirmed.";
    }
}
   