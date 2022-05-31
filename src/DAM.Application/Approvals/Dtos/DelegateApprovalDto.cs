using DAM.Application.Accounts.Dtos;
using DAM.Application.CountryRegions.Dtos;
using DAM.Application.Folders.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Dtos
{
    public class DelegateApprovalDto
    {
        public int ApprovalLevelId { get; set; }

        public int ApprovalLevelApproverId { get; set; }

        public string ApproverId { get; set; }

        public string DelegateId { get; set; }
    }
}
