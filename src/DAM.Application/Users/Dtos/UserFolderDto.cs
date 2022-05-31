using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Users.Dtos
{
    public class UserFolderDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? FolderId { get; set; }
    }
}
