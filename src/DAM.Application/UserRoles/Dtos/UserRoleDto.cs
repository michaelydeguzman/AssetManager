using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.UserRoles.Dtos
{
    public class UserRoleDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool CanArchive { get; set; }

        public bool CanEdit { get; set; }

        public bool CanShare { get; set; }

        public bool CanApprove { get; set; }

        public bool CanAdd { get; set; }

        public bool CanDelete { get; set; }

        public bool CanUpload { get; set; }

        public bool CanMove { get; set; }

        public bool CanInvite { get; set; }

        public bool CanAccessAdmin { get; set; }
        public bool CanShareFolders { get; set; }
    }
}
