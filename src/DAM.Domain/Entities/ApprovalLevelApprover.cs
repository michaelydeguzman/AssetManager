using DAM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAM.Domain.Entities
{
    public class ApprovalLevelApprover : EntityBase
    {
        public int ApprovalLevelId { get; set; }
        public ApprovalLevel ApprovalLevel { get; set; }
        
        // Old
        //public int ApproverId { get; set; }
        //public User Approver { get; set; }

        // Identity
        public string ApproverId { get; set; }
        public ApplicationUser Approver { get; set; }

        public int ApprovalStatus { get; set; }
        public DateTimeOffset? ReviewDate { get; set; }
        public string Comments { get; set; }
    }
}
