using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Users.Dtos
{
    public class InviteNewUserDto
    {
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public int UserRoleId { get; set; }
        public int? CompanyId { get; set; }
        public List<int> RootFolderIds { get; set; }
        public string Password { get; set; }
    }
}
