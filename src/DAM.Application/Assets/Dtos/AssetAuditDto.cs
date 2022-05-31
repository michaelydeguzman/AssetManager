using DAM.Application.Accounts.Dtos;
using DAM.Application.CountryRegions.Dtos;
using DAM.Application.Folders.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets.Dtos
{
    public class AssetAuditDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Comments { get; set; }
        public string Extension { get; set; }
   
        public int Size { get; set; }
        public string FileSizeText { get; set; }
        public string FileType { get; set; }

        public string Key { get; set; }

        public List<TagDto> Tags { get; set; }

        public FolderDto Folder { get; set; }

        public List<AccountDto> Accounts { get; set; }
        public List<CountryDto> Countries { get; set; }
        public List<RegionDto> Regions { get; set; }

        public string CreatedById { get; set; }

        public string CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }

        public string ModifiedById { get; set; }
        public string ModifiedBy { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }

        public string LastViewedBy { get; set; }
        public DateTimeOffset? LastViewedDate { get; set; }

        public string LastDownloadBy{ get; set; }
        public DateTimeOffset? LastDownloadDate { get; set; }

        public DateTimeOffset? ExpiryDate { get; set; }

        public string Status { get; set; }
        public DateTimeOffset? StatusUpdatedDate { get; set; }




    }
}
