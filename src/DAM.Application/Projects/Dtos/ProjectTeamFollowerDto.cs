using System;

namespace DAM.Application.Projects.Dtos
{
    public class ProjectTeamFollowerDto
    {
        public int? Id { get; set; }
        public int TeamId { get; set; }
        public int ProjectId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
        public string ModifiedById { get; set; }
    }
}
