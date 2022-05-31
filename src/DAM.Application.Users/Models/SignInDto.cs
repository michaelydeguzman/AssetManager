using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Users.Models
{
    public class SignInDto
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
        public bool IsDynamics { get; set; }
    }
}
