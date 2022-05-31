using DAM.Application.Accounts.Dtos;
using DAM.Application.CountryRegions.Dtos;
using DAM.Application.Folders.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets.Dtos
{
    public class AssetDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Comments { get; set; }
        public string Extension { get; set; }
   
        public byte[] FileBytes { get; set; }
        public int Size { get; set; }
        public string FileSizeText { get; set; }
        public string FileType { get; set; }

        public string Key { get; set; }
        public string Thumbnail { get; set; }
        public string OriginalUrl { get; set; }
        public string DownloadUrl { get; set; }
        public string CopyUrl { get; set; }

        public List<TagDto> Tags { get; set; }

        public int FolderId { get; set; }
        public FolderDto Folder { get; set; }
        public string FolderName { get; set; }
        public List<AccountDto> Accounts { get; set; }
        public List<CountryDto> Countries { get; set; }
        public List<RegionDto> Regions { get; set; }

        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }

        public string ModifiedById { get; set; }
        public string ModifiedByName { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }

        public string LastViewedBy { get; set; }
        public string LastViewedByName { get; set; }
        public DateTimeOffset? LastViewedDate { get; set; }

        public string LastDownloadBy { get; set; }
        public string LastDownloadByName { get; set; }
        public DateTimeOffset? LastDownloadDate { get; set; }

        public DateTimeOffset? ExpiryDate { get; set; }

        public int Status { get; set; }
        public string StatusName { get; set; }
        public DateTimeOffset? StatusUpdatedDate { get; set; }

        public int DownloadCount { get; set; }
        public int Version { get; set; }

        public string ShareFolderIds { get; set; }

        public List<AssetVersionsDto> AssetVersions { get; set; }

        public DateTimeOffset? DueDate { get; set; }

        public string OwnerProfilePic { get; set; }

        public string CurrentApprovalLevelNumber { get; set; }

        public int? ProjectId { get; set; }
        public string PackageUrl { get; set; }
        public string PackageName { get; set; }
        public string PackageExtension { get; set; }
        public string ThumbnailPreview { get; set; }
    }
}
