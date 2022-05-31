using System;
using System.Collections.Generic;

namespace DAM.Domain.Entities
{
    public class Asset : EntityBase
    {
        public string Title { get; set; }

        public int? FolderId { get; set; }

        public Folder Folder { get; set; }

        public int Status { get; set; }

        public DateTimeOffset? StatusUpdatedDate { get; set; }

        public DateTimeOffset? ApprovalDueDate { get; set; }

        public string Description { get; set; }

        public DateTimeOffset? ExpiryDate { get; set; }

        public String ShareFolderIds { get; set; }

        public int? ProjectId { get; set; }

        public Project Project { get; set; }
        public string PackageName { get; set; }
        public string PackageUrl { get; set; }
        public string PackageExtension { get; set; }

        public virtual ICollection<AssetAccountMetaData> AssetAccounts { get; set; }

        public virtual ICollection<AssetCountryRegionMetaData> AssetCountryRegions { get; set; }

        public virtual ICollection<Comment> AssetComments { get; set; }
        public virtual ICollection<ApprovalLevel> ApprovalLevels{ get; set; }
        public virtual ICollection<AssetVersions> AssetVersions { get; set; }
    }
}
