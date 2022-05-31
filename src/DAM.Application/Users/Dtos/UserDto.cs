using DAM.Application.Users.Constants;
using DAM.Domain.Entities;
using System;
using System.Collections.Generic;

namespace DAM.Application.Users.Dtos
{
    public class UserDto
    {
        public UserDto()
        {
            UserName = "";
            Email = "";
        }

        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        //public string Password { get; set; }
        public int? UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
        public bool Active { get; set; }
        public string ImageUrl { get; set; }
        public byte[] FileBytes { get; set; }
        public string ImageFileExtension { get; set; }
        public virtual ICollection<UserFolder> UserFolders { get; set; }

        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<string> UserIds { get; set; }
        public string FolderId { get; set; }

    }
}
