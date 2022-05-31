using DAM.Application.Accounts.Dtos;
using DAM.Application.CountryRegions.Dtos;
using DAM.Application.Folders.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Dtos
{
    public class DelegateAssetsForApprovalDto
    {
        public List<DelegateApprovalDto> DelegateApprovals { get; set; }
    }
}
