using DAM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Domain.Entities
{
    public class AssetAudit
    {
        public int? Id { get; set; }

        public int? AssetId { get; set; }

        public Asset Asset { get; set; }

        public string AssetFileName { get; set; }

        public int? FolderId { get; set; }

        public Folder Folder { get; set; }

        public string FolderName { get; set; }

        public int AuditType { get; set; }

        public string AuditTypeText { get; set; }

        public string PreviousParameters { get; set; }
        public string NewParameters { get; set; }

        public string AuditCreatedByUserId { get; set; }
        public ApplicationUser AuditCreatedByUser { get; set; }

        public DateTimeOffset AuditCreatedDate { get; set; }

        public string AuditCreatedByName { get; set; }
    }
}
