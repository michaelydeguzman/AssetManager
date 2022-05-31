using DAM.Application.Assets.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.CountryRegions.Dtos
{
    public class RegionDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int CountryId { get; set; }
        public int AssetId { get; set; }
        //public CountryDto Country { get; set; }

        //public List<AssetCountryRegionMetaDataDto> AssetRegions { get; set; }
        //public List<FolderCountryRegionMetaDataDto> FolderRegions { get; set; }
    }
}
