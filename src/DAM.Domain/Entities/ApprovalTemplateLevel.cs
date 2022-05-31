using DAM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAM.Domain.Entities
{
    public class ApprovalTemplateLevel : EntityBase
    {
        public int ApprovalTemplateId { get; set; }
        public ApprovalTemplate ApprovalTemplate { get; set; }

        public int LevelOrderNumber { get; set; }

        public virtual ICollection<ApprovalTemplateLevelApprover> Approvers { get; set; }
    }
}
