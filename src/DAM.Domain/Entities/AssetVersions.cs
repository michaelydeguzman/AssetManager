using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Domain.Entities
{
    public class AssetVersions : EntityBase
    {
        public int AssetId { get; set; }

        public string FileName { get; set; }

        public string Key { get; set; }

        public string Extension { get; set; }

        public int Status { get; set; }

        public int ActiveVersion { get; set; }
        public int DownloadCount { get; set; }

        public DateTimeOffset? StatusUpdatedDate { get; set; }

        public string Thumbnail { get; set; }

        public string Description { get; set; }

        public string DownloadUrl { get; set; }

        public long Size { get; set; }

        public string FileSizeText { get; set; }

        public string FileType { get; set; }

        public int Version { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }

        public string OriginalUrl { get; set; }

        public uint CRC32Code { get; set; }
        public string ThumbnailPreview { get; set; }
    }
}
