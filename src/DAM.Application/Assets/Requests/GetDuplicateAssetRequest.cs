using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Assets.Requests
{
    public class GetDuplicateAssetRequest : IRequest<HandlerResult<AssetDto>>
    {
        public AssetDto Asset { get; private set; }

        public GetDuplicateAssetRequest(AssetDto asset)
        {
            Asset = asset;
        }
    }
}

