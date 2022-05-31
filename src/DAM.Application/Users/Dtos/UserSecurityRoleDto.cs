
using DAM.Application.Users.Constants;
using System;
using System.Collections.Generic;

namespace DAM.Application.Users.Dtos
{
    public class UserSecurityRoleDto
    {
        public int UserId { get; set; }

        public int SecurityRoleId { get; set; }

        public bool Deleted { get; set; }
    }
}
