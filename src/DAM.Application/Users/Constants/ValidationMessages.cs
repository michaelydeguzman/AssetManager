using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Users.Constants
{
    public static class ValidationMessages
    {
        public const string UserIsExisting = "Email address is already in use.";
        public const string InvalidEmail = "Email address is not found.";
        public const string InvalidPassword = "You have entered incorrect password.";
        public const string EmailAwaitingApproval = "Email address is currently awaiting for approval.";
        public const string EmailAwaitingVerification = "Please verify your email with code sent.";
        public const string EmailAlreadyVerified = "Email address is already verified.";
        public const string InvalidVerificationCode = "Invalid Verification Code.";
        public const string NoActiveVerifications = "No active verifications as of this moment.";
        public const string InactiveEmail = "This email is currently inactive.";
    }
}
