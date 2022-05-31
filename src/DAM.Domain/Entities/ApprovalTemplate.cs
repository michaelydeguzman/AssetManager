using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAM.Domain.Entities
{
    public class ApprovalTemplate : EntityBase
    {
        public string TemplateName { get; set; }

        public bool isDeleted { get; set; }

        public int? CompanyId { get; set; }

        public virtual ICollection<ApprovalTemplateLevel> ApprovalTemplateLevels { get; set; }
    }
}
