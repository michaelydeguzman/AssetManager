using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Domain.Entities
{
    public class ProjectComment : EntityBase
    {
        public string Comment { get; set; }
        public bool Deleted { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
