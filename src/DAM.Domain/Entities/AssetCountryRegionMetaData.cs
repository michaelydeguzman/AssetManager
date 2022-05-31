using System;
using System.Collections.Generic;

namespace DAM.Domain.Entities
{
    public class AssetCountryRegionMetaData : EntityBase
    {
        public int AssetId { get; set; }
        public Asset Asset { get; set; }

        public int CountryId { get; set; }
        public Country Country { get; set; }

        public int RegionId { get; set; }
        public Region Region { get; set; }
    }
}
