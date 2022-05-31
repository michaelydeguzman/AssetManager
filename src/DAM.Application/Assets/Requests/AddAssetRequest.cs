using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Assets.Requests
{
    public class AddAssetRequest : IRequest<HandlerResult<AssetDto>>
    {
        public AssetDto AssetDto { get; private set; }

        public string UserId { get; private set; }

        public AddAssetRequest(AssetDto asset, string userId)
        {
            AssetDto = asset;
            UserId = userId;
        }
    }
}

