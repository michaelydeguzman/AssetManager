using DAM.Application.Assets.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Assets.Requests
{
    public class GetAllAssetVersionsCRC32Request : IRequest<HandlerResult<string>>
    {
        public GetAllAssetVersionsCRC32Request() { }
    }
}
