using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DAM.Application.AuditTrail.Enums
{
    public enum AssetAuditType
    {
        [Description("Asset Uploaded")]
        AssetUpload = 1,
        [Description("Asset Updated")]
        AssetUpdated = 2,
        [Description("Asset Archived")]
        AssetArchived = 3,
        [Description("Asset Expired")]
        AssetExpired = 4,
        [Description("Asset Deleted")]
        AssetDeleted = 5,
        [Description("Asset Restored")]
        AssetRestored = 6,
        [Description("Asset Downloaded")]
        AssetDownloaded = 7,
        [Description("Asset Shared")]
        AssetShared = 8,
        [Description("Asset Moved")]
        AssetMoved = 9,
        [Description("Folder Created")]
        FolderCreated = 10,
        [Description("Folder Moved")]
        FolderMoved = 11,
        [Description("Folder Deleted")]
        FolderDeleted = 12,
        [Description("Folder Updated")]
        FolderUpdated = 13,
        [Description("Asset Submitted for Review")]
        AssetSubmitted = 14,
        [Description("Asset Approved")]
        AssetApproved = 15,
        [Description("Asset Rejected")]
        AssetRejected = 16,
        [Description("Asset Version Uploaded ")]
        AssetVersionUpload = 17,
        [Description("Folder Copied")]
        FolderCopied = 18,
        [Description("Theme Changed")]
        ThemeChanged = 19,
        [Description("Logo Changed")]
        LogoChanged = 20,
        [Description("Theme Added")]
        ThemeAdded = 21,
        [Description("Package Added")]
        PackageAdded = 22,
        [Description("Asset Package Downloaded")]
        AssetPackageDownloaded = 23,
        [Description("Add Asset Preview ")]
        AddAssetPreview = 24,
        [Description("Remove Asset Preview")]
        RemoveAssetPreview = 25,
    }
}
