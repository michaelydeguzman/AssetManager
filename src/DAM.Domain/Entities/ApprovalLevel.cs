using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAM.Domain.Entities
{
    public class ApprovalLevel : EntityBase
    {
        public int AssetId { get; set; }
        public Asset Asset { get; set; }
        public int AssetVersionId { get; set; }
        public int LevelNumber { get; set; }
        public DateTimeOffset? CompletedDate { get; set; }
        public bool? IsActiveLevel { get; set; }
        public int? Duration { get; set; }
        public DateTimeOffset? DueDate { get; set; }

        public virtual ICollection<ApprovalLevelApprover> Approvers {get; set;}
    }
}
