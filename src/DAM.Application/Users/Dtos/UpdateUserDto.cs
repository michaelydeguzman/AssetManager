using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Users.Dtos
{
    public class UpdateUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public int UserRoleId { get; set; }
        public int? Company { get; set; }
        public List<int> RootFolderIds { get; set; }
        public bool Status { get; set; }
    }
}
