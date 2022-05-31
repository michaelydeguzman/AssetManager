using DAM.Application.Accounts.Dtos;
using DAM.Application.CountryRegions.Dtos;
using System;
using System.Collections.Generic;

namespace DAM.Application.Assets.Dtos
{
    public class UpdateAssetTagsDto
    {
        public List<AssetDto> AssetsToUpdate { get; set; }
    }
}
