using DAM.Application.Approval.Dtos;
using DAM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.AuditTrail.Dtos
{
    public class AssetApprovalAuditDto
    {
        public List<ApprovalLevelDto> ApprovalLevels { get; set; }
    }
}
