using DAM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAM.Domain.Entities
{
    public class ApprovalTemplateLevelApproverDto
    {
        public int? Id { get; set; }

        public string ApproverId { get; set; }
    }
}
