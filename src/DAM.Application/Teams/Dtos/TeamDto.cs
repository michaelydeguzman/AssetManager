using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Teams.Dtos
{
    public class TeamDto
    {
        public int? Id { get; set; }
        public string TeamName { get; set; }
        public string TeamDescription { get; set; }
        public bool Approvals { get; set; }
        public bool Project { get; set; }
        public int Status { get; set; }
        public bool Deleted { get; set; }
        public string CreatedById { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string ModifiedById { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }

        public List<TeamMemberDto> TeamMembers { get; set; }

        public List<string> TeamMemberUserIds { get; set; }
    }
}
