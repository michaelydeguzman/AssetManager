using DAM.Application.Accounts.Dtos;
using DAM.Application.CountryRegions.Dtos;
using DAM.Application.Folders.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Approvals.Dtos
{
    public class ApprovalsOnOOODto
    {
        public int AssetId { get; set; }

        public int AssetVersionId { get; set; }

        public string Title { get; set; }
        public string FileName { get; set; }
        public int Version { get; set; }

        public int LevelNumber { get; set; }

        public DateTimeOffset DueDate { get; set; }

        public int ApprovalLevelId { get; set; }

        public int ApprovalLevelApproverId { get; set; }

        public string ApproverId { get; set; }

        public int? FolderId { get; set; }

        public int? CompanyId { get; set; }
    }
}
