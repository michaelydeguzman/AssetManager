using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Domain.Entities
{
    public class ProjectItem : EntityBase
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int AssetId { get; set; }
        public Asset Asset { get; set; }
    }
}
