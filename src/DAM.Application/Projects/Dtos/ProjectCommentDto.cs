using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Projects.Dtos
{
    public class ProjectCommentDto
    {
        public int? Id { get; set; }
        public string Comment { get; set; }
        public bool Deleted { get; set; }
        public int ProjectId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
        public string ModifiedById { get; set; }

        public List<string> Mentions { get; set; }
    }
}
