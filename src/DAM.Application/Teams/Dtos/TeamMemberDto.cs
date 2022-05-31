using DAM.Domain.Entities.Identity;
using System;

namespace DAM.Application.Teams.Dtos
{
    public class TeamMemberDto
    {
        public int? Id { get; set; }
        public string UserId { get; set; }
        public int TeamId { get; set; }

        public string CreatedById { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string ModifiedById { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
