using DAM.Application.Companies.Dtos;
using DAM.Application.Watermarks.Dto;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Watermarks.Requests
{
    public class GetDefaultWatermarkRequest : IRequest<HandlerResult<WatermarkDto>>
    {
        public GetDefaultWatermarkRequest()
        {
        }
    }
}
