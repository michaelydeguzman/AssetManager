using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Assets.Requests
{
    public class GetArchiveRequest : IRequest<HandlerResult<IEnumerable<AssetDto>>>
    {
        public int AssetId { get; private set; }
        public int ExpiryDays { get; private set; }

        public GetArchiveRequest(int assetId, int expiryDays)
        {
            AssetId = assetId;
            ExpiryDays = expiryDays;
        }
    }
}

