using DAM.Application.Accounts.Dtos;
using DAM.Application.CountryRegions.Dtos;
using System;
using System.Collections.Generic;

namespace DAM.Application.Assets.Dtos
{
    public class UpdateAssetDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }

        public bool Deleted { get; set; }

        public byte[] FileBytes { get; set; }
        public string FileType { get; set; }
        public string Extension { get; set; }

        public List<AccountDto> Accounts { get; set; }
        public List<CountryDto> Countries { get; set; }
        public List<RegionDto> Regions { get; set; }

        public List<TagDto> Tags { get; set; }

        public string DownloadUrl { get; set; }

        public DateTimeOffset? ExpiryDate { get; set; }

        public string CreatedById { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string ModifiedById { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }

        public string ShareFolderIds { get; set; }
    }
}
