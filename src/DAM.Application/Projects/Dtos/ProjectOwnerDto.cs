using System;

namespace DAM.Application.Projects.Dtos
{
    public class ProjectOwnerDto
    {
        public int? Id { get; set; }
        public string UserId { get; set; }
        public int ProjectId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
        public string ModifiedById { get; set; }
    }
}
