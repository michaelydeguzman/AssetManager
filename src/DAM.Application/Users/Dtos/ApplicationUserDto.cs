using DAM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Users.Dtos
{
    public class ApplicationUserDto
    {
        // Extension Properties
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ImageFileExtension { get; set; }
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
        public bool Deleted { get; set; }
        public bool Active { get; set; }
        public int? CompanyId { get; set; }
        public Company Company { get; set; }
        public string CompanyName { get; set; }
        public string ImageUrl { get; set; }
        public virtual ICollection<UserFolder> UserFolders { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
