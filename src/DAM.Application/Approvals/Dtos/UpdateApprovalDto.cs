using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Dtos
{
    public class UpdateApprovalDto
    {
        public int AssetId { get; set; }
        public int AssetVersionId { get; set; }
        public string ApproverId { get; set; }
        public bool IsApproved { get; set; }
        public string Comment { get; set; }
    }
}
