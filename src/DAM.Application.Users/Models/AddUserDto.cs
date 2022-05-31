using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Users.Models
{
    public class AddUserDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string  Email { get; set; }

        public string Password { get; set; }

        public int RoleId { get; set; }
    }
}
