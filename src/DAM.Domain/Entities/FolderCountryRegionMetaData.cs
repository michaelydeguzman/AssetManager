using System;
using System.Collections.Generic;

namespace DAM.Domain.Entities
{
    public class FolderCountryRegionMetaData : EntityBase
    {
        public int FolderId { get; set; }
        public Folder Folder { get; set; }

        public int CountryId { get; set; }
        public Country Country { get; set; }

        public int RegionId { get; set; }
        public Region Region { get; set; }
    }
}
