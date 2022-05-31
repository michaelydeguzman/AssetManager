using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.AuditTrail.Dtos
{
    public class AuditTrailDto
    {
        public int TotalCount { get; set; }

        public List<AssetAuditTrailDto> AssetAuditTrail { get; set; }
    }
}
