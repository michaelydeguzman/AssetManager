using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Users.Models
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }

        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }

        public string Token { get; set; }
    }
}
