using DAM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approval.Dtos
{
    public class ApprovalLevelDto
    {
        public int? Id { get; set; }

        public int ApprovalLevelStatus { get; set; }

        public int AssetId { get; set; }

        public int AssetVersionId { get; set; }

        public int LevelNumber { get; set; }

        public bool isActiveLevel { get; set; }

        public int? Duration { get; set; }

        public DateTimeOffset? DueDate { get; set; }

        public List<ApprovalLevelApproverDto> Approvers { get; set; }

        public DateTimeOffset? CompletedDate { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public string CreatedById { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }

        public string ModifiedById { get; set; }

        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }


    }
}
