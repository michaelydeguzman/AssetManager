using DAM.Application.Approval.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Dtos
{
    public class AssetApprovalDto
    {
        public int AssetId { get; set; }
        public int AssetVersionId { get; set; }
        public List<ApprovalLevelDto> ApprovalLevels { get; set; }
        public bool isSubmitted { get; set; }
        public string CreatedById { get; set; }
        public string ModifiedById { get; set; }

        public bool DelegateIfOOO { get; set; }
    }
}
