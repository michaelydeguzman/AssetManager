using DAM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAM.Domain.Entities
{
    public class ApprovalTemplateLevelDto
    {
        public int? Id { get; set; }

        public int LevelOrderNumber { get; set; }

        public virtual ICollection<ApprovalTemplateLevelApproverDto> Approvers { get; set; }
    }
}
