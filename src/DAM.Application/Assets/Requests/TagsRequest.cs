using DAM.Application.Assets.Dtos;
using DAM.Application.Users.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Assets.Requests
{
    public class TagsRequest : IRequest<HandlerResult<IEnumerable<TagDto>>>
    {

    }
}

