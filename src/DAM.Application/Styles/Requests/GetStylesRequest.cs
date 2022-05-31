using DAM.Application.Styles.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Styles.Requests
{
    public class GetStylesRequest : IRequest<HandlerResult<IEnumerable<ThemeDto>>>
    {

        public GetStylesRequest()
        {

        }
    }
}
