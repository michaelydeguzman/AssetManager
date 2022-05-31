using DAM.Application.Accounts.Dtos;
using DAM.Application.CountryRegions.Dtos;
using DAM.Application.Folders.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Dtos
{
    public class GetApprovalsOnOOODto
    {
        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public string ApproverId { get; set; }
    }
}
