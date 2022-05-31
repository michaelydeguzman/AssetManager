using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Assets.Requests
{
    public class MoveAssetsRequest : IRequest<HandlerResult<MoveAssetsDto>>
    {
        public string UserId { get; private set; }
        public MoveAssetsDto MoveAssets { get; private set; }

        public MoveAssetsRequest(MoveAssetsDto moveAssets, string userId)
        {
            MoveAssets = moveAssets;
            UserId = userId;
        }
    }
}