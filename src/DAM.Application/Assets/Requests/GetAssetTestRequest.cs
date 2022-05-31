using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Assets.Requests
{
    public class GetAssetTestRequest : IRequest<HandlerResult<IEnumerable<AssetDto>>>
    {
        public string Type { get; private set; }

        public GetAssetTestRequest(string type)
        {
            Type = type;
        }
    }
}

