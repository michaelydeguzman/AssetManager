using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Users.Models
{
    public class JwtToken
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
