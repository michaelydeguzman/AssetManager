using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using DAM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.AuditTrail.Dtos
{
    public class AssetAuditTrailDto
    {
        
        public int? Id { get; set; }

        public int? AssetId { get; set; }

        public string AssetFileName { get; set; }

        public string FolderName { get; set; }

        public int AuditType { get; set; }

        public string AuditTypeText { get; set; }

        public string PreviousParameters { get; set; }
        public string NewParameters { get; set; }

        public string AuditCreatedByUserId { get; set; }
        public string AuditCreatedByName { get; set; }
        public DateTimeOffset AuditCreatedDate { get; set; }
    }
}
