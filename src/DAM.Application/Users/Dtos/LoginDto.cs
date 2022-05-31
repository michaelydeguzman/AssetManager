using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAM.Application.Users.Dtos
{
    public class LoginDto
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public bool Accepted { get; set; }

        public string ErrorMessage { get; set; }

        public string DisplayName { get; set; }

        public int UserId { get; set; } = 0;
    }
}
