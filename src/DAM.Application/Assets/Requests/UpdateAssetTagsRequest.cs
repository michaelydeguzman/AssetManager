using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Assets.Requests
{
    public class UpdateAssetTagsRequest : IRequest<HandlerResult<List<AssetDto>>>
    {
        public string UserId { get; private set; }
        public List<AssetDto> AssetsToUpdate { get; private set; }

        public UpdateAssetTagsRequest(List<AssetDto> assetsToUpdate, string userId)
        {
            AssetsToUpdate = assetsToUpdate;
            UserId = userId;
        }
    }
}