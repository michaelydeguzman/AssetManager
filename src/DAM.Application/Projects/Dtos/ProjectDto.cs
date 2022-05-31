using System;
using System.Collections.Generic;

namespace DAM.Application.Projects.Dtos
{
    public class ProjectDto
    {
        public int? Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectOverview { get; set; }
        public int ProjectStatus { get; set; }
        public DateTimeOffset? ProjectDueDate { get; set; }

        public List<ProjectUserFollowerDto> ProjectUserFollowers { get; set; }
        public List<ProjectTeamFollowerDto> ProjectTeamFollowers { get; set; }
        public List<ProjectOwnerDto> ProjectOwners { get; set; }
        public List<ProjectItemDto> ProjectItems { get; set; }
        public List<ProjectCommentDto> ProjectComments { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
        public string ModifiedById { get; set; }

        public List<int> ProjectUploads { get; set; }
    }
}
