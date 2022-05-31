using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Domain.Entities
{
    public class AssetAccountMetaData : EntityBase
    {
        public int AssetId { get; set; }
        public Asset Asset { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
