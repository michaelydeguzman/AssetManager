using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Domain.Entities.Identity
{
    public class ApplicationRole : IdentityRole
    {
        // Extension Properties
        public bool CanArchive { get; set; }

        public bool CanEdit { get; set; }

        public bool CanShare { get; set; }

        public bool CanApprove { get; set; }

        public bool CanAdd { get; set; }
        public bool CanDelete { get; set; }
        public bool CanUpload { get; set; }
        public bool CanMove { get; set; }
        public bool CanInvite { get; set; }

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }
    }
}
