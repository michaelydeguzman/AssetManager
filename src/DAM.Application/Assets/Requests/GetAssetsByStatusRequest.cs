using DAM.Application.Assets.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Assets.Requests
{
    public class GetAssetsByStatusRequest : IRequest<HandlerResult<IEnumerable<AssetDto>>>
    {
        public int AssetStatus { get; private set; }

        public GetAssetsByStatusRequest (int assetStatus)
        {
            AssetStatus = assetStatus;
        }
    }
}
