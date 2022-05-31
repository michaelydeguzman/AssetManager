using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approval.Dtos
{
    public class ApprovalLevelApproverDto
    {
        public int? Id { get; set; }
        public int ApprovalLevelId { get; set; }
        //public ApprovalLevel ApprovalLevel { get; set; }
        public string ApproverId { get; set; }
        public string ApproverName { get; set; }
        //public ApplicationUser Approver { get; set; }
        public int ApprovalStatus { get; set; }
        public DateTimeOffset? ReviewDate { get; set; }
        public string Comments { get; set; }
    }
}
