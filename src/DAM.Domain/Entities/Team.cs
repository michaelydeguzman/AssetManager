using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Domain.Entities
{
    public class Team : EntityBase
    {
        public string TeamName { get; set; }
        public string TeamDescription { get; set; }
        public bool Approvals { get; set; }
        public bool Project { get; set; }
        public int Status { get; set; }

        public bool Deleted { get; set; }

        public virtual ICollection<TeamMember> TeamMembers { get; set; }
    }
}
