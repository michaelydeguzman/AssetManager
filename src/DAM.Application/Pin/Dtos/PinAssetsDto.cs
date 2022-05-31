using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Pin.Dtos
{
    public class PinAssetsDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int AssetId { get; set; }
        public int OrderNumber { get; set; }
        public int NewAssetId { get; set; }
        public int PreAssetId { get; set; }
        public List<int> OrderedAssetIds { get; set; }
    }
}
