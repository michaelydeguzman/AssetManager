using DAM.Application.Logos.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Logos.Requests
{
    public class GetLogosRequest : IRequest<HandlerResult<IEnumerable<LogoDto>>>
    {

        public GetLogosRequest()
        {

        }
    }
}
