using DAM.Application.Companies.Dtos;
using DAM.Application.Watermarks.Dto;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Watermarks.Requests
{
    public class SaveDefaultWatermarkRequest : IRequest<HandlerResult<WatermarkDto>>
    {
        public string UserId { get; set; }

        public WatermarkDto DefaultWatermark { get; set; }

        public SaveDefaultWatermarkRequest(WatermarkDto defaultWatermark, string userId)
        {
            UserId = userId;
            DefaultWatermark = defaultWatermark;
        }
    }
}
