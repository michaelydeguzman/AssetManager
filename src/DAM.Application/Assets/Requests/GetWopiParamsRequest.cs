using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Assets.Requests
{
    public class GetWopiParamsRequest : IRequest<HandlerResult<WopiParamDto>>
    {
        public string AssetKey { get; private set; }

        public int Action { get; private set; }
        public GetWopiParamsRequest(string assetKey, int action)
        {
            AssetKey = assetKey;
            Action = action;
        }
    }
}

