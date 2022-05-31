using DAM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAM.Domain.Entities
{
    public class ApprovalTemplateLevelApprover : EntityBase
    {
        public int ApprovalTemplateLevelId { get; set; }
        public ApprovalTemplateLevel ApprovalTemplateLevel { get; set; }

        public string ApproverId { get; set; }
        public ApplicationUser Approver { get; set; }
    }
}
