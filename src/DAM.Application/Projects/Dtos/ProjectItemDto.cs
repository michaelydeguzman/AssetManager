using System;

namespace DAM.Application.Projects.Dtos
{
    public class ProjectItemDto
    {
        public int? Id { get; set; }
        public int ProjectId { get; set; }
        public int AssetId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
        public string ModifiedById { get; set; }
    }
}
