using DAM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Domain.Entities
{
    public class Delegation : EntityBase
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
   
        public string DelegateId { get; set; }

        public int ApprovalLevelId { get; set; }

        public int ApprovalLevelApproverId { get; set; }
    }
}
