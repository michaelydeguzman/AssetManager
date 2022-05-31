using DAM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Domain.Entities
{
    public class ProjectOwner : EntityBase
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
