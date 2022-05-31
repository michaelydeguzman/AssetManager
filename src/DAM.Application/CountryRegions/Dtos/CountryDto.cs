using DAM.Application.Assets.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.CountryRegions.Dtos
{
    public class CountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AssetId { get; set; }

        public virtual ICollection<RegionDto> Regions { get; set; }
        //public List<AssetCountryRegionMetaDataDto> AssetCountries { get; set; }
        //public List<FolderCountryRegionMetaDataDto> FolderCountries { get; set; }
    }
}
