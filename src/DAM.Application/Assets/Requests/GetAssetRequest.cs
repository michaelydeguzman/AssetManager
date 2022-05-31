using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Assets.Requests
{
    public class GetAssetRequest : IRequest<HandlerResult<IEnumerable<AssetDto>>>
    {
        public int AssetId { get; private set; }

        public GetAssetRequest(int assetId)
        {
            AssetId = assetId;
        }
    }
}

