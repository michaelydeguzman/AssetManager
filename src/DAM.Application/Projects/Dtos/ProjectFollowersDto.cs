using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Projects.Dtos
{
    public class ProjectFollowersDto
    {
        public int ProjectId { get; set; }
        public List<ProjectUserFollowerDto> Users { get; set; }

        public List<ProjectTeamFollowerDto> Teams { get; set; }
    }
}
