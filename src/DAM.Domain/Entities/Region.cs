using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAM.Domain.Entities
{
    public class Region
    {
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }

        public int CountryId { get; set; }

        public Country Country { get; set; }

        public virtual ICollection<AssetCountryRegionMetaData> AssetRegions { get; set; }

        public virtual ICollection<FolderCountryRegionMetaData> FolderRegions { get; set; }
    }
}
