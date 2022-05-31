
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAM.Domain.Entities
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Region> Regions { get; set; }

        public virtual ICollection<AssetCountryRegionMetaData> AssetCountries { get; set; }

        public virtual ICollection<FolderCountryRegionMetaData> FolderCountries { get; set; }
    }
}
