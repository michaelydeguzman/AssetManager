using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAM.Domain.Entities
{
    public class ApprovalTemplateDto
    {
        public int? Id { get; set; }

        public string TemplateName { get; set; }

        public bool isDeleted { get; set; }

        public int? CompanyId { get; set; }

        public virtual ICollection<ApprovalTemplateLevelDto> ApprovalTemplateLevels { get; set; }
    }
}
