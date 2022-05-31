using DAM.Application.CountryRegions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets.Dtos
{
    public class AssetCountryRegionMetaDataDto
    {
        public int? Id { get; set; }

        public int AssetId { get; set; }
        public AssetDto Asset { get; set; }
        public int CountryId { get; set; }
        public CountryDto Country { get; set; }
        public int Regionid { get; set; }
        public RegionDto Region { get; set; }

        public string CreatedById { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string ModifiedById { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
