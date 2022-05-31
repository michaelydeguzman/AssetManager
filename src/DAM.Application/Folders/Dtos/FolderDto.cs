using DAM.Application.Accounts.Dtos;
using DAM.Application.Assets;
using DAM.Application.Assets.Dtos;
using DAM.Application.CountryRegions.Dtos;
using DAM.Application.Companies.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Folders.Dtos
{
    public class FolderDto
    {
        public int? Id { get; set; }
        public string FolderName { get; set; }
        public string Comments { get; set; }
        public int ParentFolderId { get; set; }
        public bool Deleted { get; set; }

        public ICollection<AssetDto> Assets { get; set; }
        //public ICollection<FolderDto> Subfolders { get; set; }
        public ICollection<FolderAccountMetaDataDto> FolderAccounts { get; set; }
        public ICollection<FolderCountryRegionMetaDataDto> FolderCountryRegions { get; set; }

        public ICollection<AccountDto> Accounts { get; set; }
        public ICollection<CountryDto> Countries { get; set; }
        public ICollection<RegionDto> Regions { get; set; }

        public List<CommentDto> FolderComments { get; set; }

        public string CreatedById { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string ModifiedById { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
        public ICollection<CompanyDto> Company { get; set; }

        public int AssetCount { get; set; }

        public int OrderNumber { get; set; }
    }
}
