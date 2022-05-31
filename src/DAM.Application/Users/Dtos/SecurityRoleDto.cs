using DAM.Application.Users.Constants;
using System;
using System.Collections.Generic;

namespace DAM.Application.Users.Dtos
{
    public class SecurityRoleDto
    {
        public int? Id { get; set; }

        public string RoleName { get; set; }

        public bool Deleted { get; set; }
    }
}
