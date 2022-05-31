using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Domain.Entities
{
    public class Project : EntityBase
    {
        public string ProjectName { get; set; }
        public string ProjectOverview { get; set; }
        public int ProjectStatus { get; set; }
        public int StatusBeforeArchive { get; set; }
        public bool Deleted { get; set; }
        public DateTimeOffset? ProjectDueDate { get; set; }

        public virtual ICollection<ProjectUserFollower> ProjectUserFollowers { get; set; }
        public virtual ICollection<ProjectTeamFollower> ProjectTeamFollowers { get; set; }
        public virtual ICollection<ProjectItem> ProjectItems { get; set; }
        public virtual ICollection<ProjectComment> ProjectComments { get; set; }
    }
}
