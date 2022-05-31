using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets.Dtos
{
    public class AssetVersionsDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Comments { get; set; }
        public string Extension { get; set; }
        public int ActiveVersion { get; set; }
        public int DownloadCount { get; set; }
        public byte[] FileBytes { get; set; }
        public int Size { get; set; }
        public string FileSizeText { get; set; }
        public string FileType { get; set; }
        public int Version { get; set; }
        public string Key { get; set; }
        public string Thumbnail { get; set; }
        public string OriginalUrl { get; set; }
        public string DownloadUrl { get; set; }
        public string CopyUrl { get; set; }

        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }

        public string ModifiedById { get; set; }
        public string ModifiedByName { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
        public List<TagDto> Tags { get; set; }
        public int Status { get; set; }
        public string ThumbnailPreview { get; set; }
    }
}

